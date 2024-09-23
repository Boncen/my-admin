using System.Data;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Nodes;
using System.Transactions;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MyAdmin.Core.Conf;
using MyAdmin.Core.Entity;
using MyAdmin.Core.Exception;
using MyAdmin.Core.Extensions;
using MyAdmin.Core.Options;
using MyAdmin.Core.Repository;
using MyAdmin.Core.Utilities;

namespace MyAdmin.Core.Framework;

public class EasyApi
{
    private readonly Core.MaFrameworkBuilder _maFrameworkBuilder;
    private readonly ICacheManager _cacheManager;
    private readonly MaFrameworkOptions _maFrameworkOptions;
    private readonly DBHelper _dbHelper;
    public EasyApi(Core.MaFrameworkBuilder maFrameworkBuilder, ICacheManager cacheManager, IOptionsSnapshot<MaFrameworkOptions> optionSnap, DBHelper dbHelper)
    {
        _maFrameworkBuilder = maFrameworkBuilder;
        _cacheManager = cacheManager;
        _maFrameworkOptions = optionSnap.Value;
        _dbHelper = dbHelper;
    }

    private List<string> GetEntityList()
    {
        var list = _cacheManager.Get(ConstStrings.MACacheKeyAllEntities);
        if (list == null || list.Count < 1)
        {
            list = new List<string>();
            foreach (var assembly in _maFrameworkBuilder.Assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsInterface)
                    {
                        continue;
                    }

                    if (typeof(IEntity).IsAssignableFrom(type) && Check.HasValue(type.FullName))
                    {
                        list.Add(type.FullName);
                    }
                }
            }

            _cacheManager.Save(ConstStrings.MACacheKeyAllEntities, list);
        }

        return list;
    }
    /// <summary>
    /// get查询请求
    /// </summary>
    /// <param name="queryCollection"></param>
    /// <param name="option"></param>
    /// <returns></returns>
    /// <exception cref="MAException"></exception>
    public EasyApiParseResult ProcessQueryRequest(IQueryCollection queryCollection)
    {
        EasyApiParseResult result = new EasyApiParseResult()
        {
            Page = 1,
            Count = 1,
            Columns = "*",
            Sql = string.Empty,
            Table = string.Empty,
        };
        if (!queryCollection.ContainsKey("@target"))
        {
            return result;
        }


        List<string> queryList = new();
        List<string> orderStrs = new();
        foreach (var q in queryCollection)
        {
            if (q.Key.Equals("@total", StringComparison.CurrentCultureIgnoreCase))
            {
                int.TryParse(q.Value.ToString(), out int total);
                result.Total = total;
                continue;
            }
            if (q.Key.Equals("@page", StringComparison.CurrentCultureIgnoreCase))
            {
                int.TryParse(q.Value.ToString(), out int page);
                result.Page = page;
                continue;
            }
            if (q.Key.Equals("@count", StringComparison.CurrentCultureIgnoreCase))
            {
                int.TryParse(q.Value.ToString(), out int count);
                result.Count = count;
                continue;
            }
            if (q.Key.Equals("@target", StringComparison.CurrentCultureIgnoreCase))
            {
                result.Target = q.Value.ToString();
                result.Table = GetTableAlias(result.Target);
                // if (!IsEntity(result.Table))
                // {
                //     result.Success = false;
                //     result.Msg = "target无效";
                //     return result;
                // }

                continue;
            }
            if (q.Key.Equals("@orderAsc", StringComparison.CurrentCultureIgnoreCase))
            {
                string val = q.Value.ToString();
                if (Check.HasValue(val))
                {
                    orderStrs.Add(GetFieldAlias(val) + " ASC");
                }
                continue;
            }
            if (q.Key.Equals("@orderDesc", StringComparison.CurrentCultureIgnoreCase))
            {
                string val = q.Value.ToString();
                if (Check.HasValue(val))
                {
                    orderStrs.Add(GetFieldAlias(val) + " DESC");
                }
                continue;
            }

            if (q.Key.Equals("@columns", StringComparison.CurrentCultureIgnoreCase) && Check.HasValue(q.Value))
            {
                result.Columns = GetQueryColumnAlias(q.Value.ToString(), _maFrameworkOptions.EasyApi!);
                if (!Check.IfSqlFragmentSafe(result.Columns))
                {
                    result.Success = false;
                    result.Msg = "输入了无效参数";
                    return result;
                }
                continue;
            }

            if (int.TryParse(q.Value.ToString(), out int v))
            {
                queryList.Add($"{q.Key}={q.Value.ToString()}");
            }
            else
            {
                queryList.Add($"{q.Key}='{q.Value.ToString()}'");
            }

        }

        string where = string.Empty;
        string order = string.Empty;
        if (queryList.Count > 0)
        {
            where += " WHERE " + string.Join(" AND ", queryList);
        }
        if (result.Total.HasValue)
        {
            result.TotalSql = $"SELECT COUNT(1) from {result.Table} {where}";
        }
        if (orderStrs.Count > 0)
        {
            order = $"ORDER BY " + string.Join(",", orderStrs);
        }
        result.Sql = $"SELECT {result.Columns} from {result.Table} {where} {order} {ProcessPagedSql(result.Page, result.Count)}";
        if (!Check.IfSqlFragmentSafe(result.Sql))
        {
            result.Success = false;
            result.Msg = "不安全的SQL";
        }
#if DEBUG
        Console.WriteLine(result.Sql);
#endif
        return result;
    }

    public EasyApiParseResult ProcessDeleteRequest(IQueryCollection queryCollection)
    {
        EasyApiParseResult result = new EasyApiParseResult()
        {
            OperationType = SqlOperationType.None
        };
        foreach (var q in queryCollection)
        {
            if (q.Key.Equals("target", StringComparison.CurrentCultureIgnoreCase))
            {
                result.Target = q.Value.ToString();
                result.Table = GetTableAlias(result.Target);
                continue;
            }
            if (q.Key.Equals("id", StringComparison.CurrentCultureIgnoreCase))
            {

                result.Sql = $"DELETE FROM {result.Table} WHERE {result.Table}.Id  = '{q.Value}' ";
                continue;
            }
            if (q.Key.Equals("ids", StringComparison.CurrentCultureIgnoreCase))
            {
                var idsArray = q.Value.ToString().Split(',');
                if (idsArray.Length < 1)
                {
                    continue;
                }

                result.Sql = $"DELETE FROM {result.Table} WHERE {result.Table}.Id  IN ({string.Join(',', idsArray.ToStringCollection())}) ";
            }

        }
        // result.Sql = $"DELETE FROM {result.Table} WHERE {result.Table}.Id  = 1 ";
        if (!Check.IfSqlFragmentSafe(result.Sql))
        {
            result.Success = false;
            result.Msg = "不安全的SQL";
        }
        return result;
    }

    /// <summary>
    /// Get Column Alias, output: id as _id, name as _name, etc
    /// </summary>
    /// <param name="columns">expect format: col1,col2</param>
    /// <param name="option"></param>
    /// <returns></returns>
    private string GetQueryColumnAlias(string columns, EasyApiOptions option)
    {
        if (option.ColumnAlias == null)
        {
            return columns;
        }

        string[] cols = columns.Split(',');
        List<string> colList = new();
        foreach (var col in cols)
        {
            if (option.ColumnAlias.ContainsKey(col))
            {
                var val = option.ColumnAlias[col];
                if (Check.HasValue(val))
                {
                    colList.Add($"{val} as {col}");
                }
            }
            else
            {
                colList.Add($"{col}");
            }
        }

        return colList.Count == 0 ? columns : string.Join(',', colList);
    }

    private List<string> GetOriginalColumn(string columns)
    {
        if (!Check.HasValue(columns))
        {
            return new List<string>();
        }
        return GetOriginalColumn(columns.Split(",").ToList());
    }
    /// <summary>
    /// 列别名转为表列名
    /// </summary>
    /// <param name="columns"></param>
    /// <param name="option"></param>
    /// <returns></returns>
    private List<string> GetOriginalColumn(List<string> columns)//, EasyApiOptions option)
    {
        var option = _maFrameworkOptions.EasyApi;
        if (option.ColumnAlias == null)
        {
            return default;
        }
        List<string> colList = new();
        foreach (var col in columns)
        {
            if (option.ColumnAlias.ContainsKey(col))
            {
                var val = option.ColumnAlias[col];
                if (Check.HasValue(val))
                {
                    colList.Add($"{val}");
                }
            }
            else
            {
                colList.Add($"{col}");
            }
        }

        return colList.Count == 0 ? columns : colList;
    }

    /// <summary>
    /// 检查表名是否为IEntity
    /// </summary>
    /// <param name="target"></param>
    /// <param name="option"></param>
    private bool IsEntity(string target)
    {
        var entities = GetEntityList();
        foreach (var entityFullName in entities)
        {
            if (!Check.HasValue(entityFullName))
            {
                continue;
            }

            if (entityFullName.ToLower().IndexOf(("." + target).ToLower()) > -1)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 获取别名
    /// </summary>
    /// <param name="target"></param>
    /// <param name="option"></param>
    private string GetTableAlias(string? target)
    {
        if (!Check.HasValue(target))
        {
            return target;
        }
        if (_maFrameworkOptions.EasyApi?.TableAlias != null)
        {
            if (_maFrameworkOptions.EasyApi.TableAlias.ContainsKey(target))
            {
                var val = _maFrameworkOptions.EasyApi.TableAlias[target];
                if (Check.HasValue(val))
                {
                    return Check.HasValue(val) ? val : target;
                }
            }
        }

        return target;
    }

    public List<JsonObject> HandleDataReader(IDataReader data, Dictionary<string, string>? columnAlias,
        string table)
    {
        DataTable dt = new DataTable();

        dt.Load(data);

        List<JsonObject> result = new List<JsonObject>();
        foreach (DataRow row in dt.Rows)
        {
            JsonObject jobj = new JsonObject();
            foreach (DataColumn dtColumn in dt.Columns)
            {
                var name = dtColumn.ColumnName;
                if (columnAlias.ContainsValue(name) || columnAlias.ContainsValue(table + "." + name))
                {
                    name = columnAlias.FirstOrDefault(x =>
                        x.Value.Equals(table + "." + name) || x.Value.Equals(name, StringComparison.CurrentCultureIgnoreCase)).Key;
                    // key前面加下划线 _ 的配置项视为忽略输出
                    if (name == "_" + dtColumn.ColumnName)
                    {
                        continue;
                    }
                }
                jobj.Add(name, row.Field<dynamic>(dtColumn));
            }

            result.Add(jobj);
        }

        return result;


    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="body"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<List<EasyApiParseResult>> ProcessPostRequest(Stream body, MaFrameworkOptions option)
    {
        List<EasyApiParseResult> results = new List<EasyApiParseResult>();

        var root = await JsonNode.ParseAsync(body);
        if (root == null)
        {
            return results;
        }

        var rootObject = root.AsObject();
        if (rootObject != null)
        {
            foreach (var tableNode in rootObject)
            {
                if (tableNode.Value == null)
                {
                    continue;
                }

                var result = new EasyApiParseResult()
                {
                    Page = 1,
                    Count = 1,
                    Columns = "*",
                    OperationType = SqlOperationType.Table
                };
                if (tableNode.Value is JsonArray)
                {
                    // POST方式 表节点下的数组现在作为批量添加
                    result = ProcessAddData(tableNode, option.EasyApi, true);
                    results.Add(result);
                    continue;
                }
                JsonObject subObject = tableNode.Value.AsObject();
                if (!subObject.Any(x => x.Key.StartsWith('@')))
                {
                    result = ProcessAddData(tableNode, option.EasyApi);
                    results.Add(result);
                    continue;
                }

                result.Target = tableNode.Key;
                result.Table = GetTableAlias(result.Target);
                // if (!IsEntity(result.Table))
                // {
                //     result.Success = false;
                //     result.Msg = "target无效";
                //     results.Add(result);
                //     return results;
                // }
                List<string> joinStrs = new();
                List<string> orderStrs = new();
                string where = "";
                foreach (var subProperty in subObject)
                {
                    if (subProperty.Key.Equals("@columns", StringComparison.CurrentCultureIgnoreCase))
                    {
                        result.Columns = GetQueryColumnAlias(subProperty.Value.ToString(), option.EasyApi);
                        if (!Check.IfSqlFragmentSafe(result.Columns))
                        {
                            result.Success = false;
                            result.Msg = "输入了无效参数";
                            results.Add(result);
                            return results;
                        }
                        continue;
                    }

                    if (subProperty.Key.Equals("@count", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (int.TryParse(subProperty.Value.ToString(), out int count))
                        {
                            result.Count = count;
                        }
                    }

                    if (subProperty.Key.Equals("@total", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (int.TryParse(subProperty.Value.ToString(), out int total))
                        {
                            result.Total = total;
                        }
                    }

                    if (subProperty.Key.Equals("@page", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (int.TryParse(subProperty.Value.ToString(), out int page))
                        {
                            result.Page = page;
                        }
                    }
                    if (subProperty.Key.Equals("@join", StringComparison.CurrentCultureIgnoreCase))
                    {
                        ProcessJoinPart(subProperty, joinStrs, result.Table);
                    }
                    if (subProperty.Key.Equals("@where", StringComparison.CurrentCultureIgnoreCase))
                    {
                        where = ProcessWhereParam(result, subProperty.Value);
                    }
                    if (subProperty.Key.Equals("@order", StringComparison.CurrentCultureIgnoreCase))
                    {
                        ProcessOrderByPart(subProperty, orderStrs, result.Table);
                    }
                    if (subProperty.Key.Equals("@children", StringComparison.CurrentCultureIgnoreCase))
                    {
                        result.Children = ProcessChildrenQuery(subProperty.Value, result.Table);

                        if (Check.HasValue(result.Children.ChildrenConfig?.ParentField) && result.Columns?.Trim() != "*" && !result.Columns.Contains(result.Children.ChildrenConfig?.ParentField))
                        {
                            result.Columns += $",{result.Children.ChildrenConfig?.ParentField}";
                        }
                    }
                } // for property
                  // 查询的情况
                string joinStr = string.Empty;
                string orderStr = string.Empty;
                if (joinStrs.Count > 0)
                {
                    joinStr = string.Join(' ', joinStrs);
                }
                if (result.Total.HasValue)
                {
                    result.TotalSql = $"SELECT COUNT(1) from {result.Table}  {joinStr} {where}";
                }
                if (orderStrs.Count > 0)
                {
                    orderStr = " ORDER BY " + string.Join(',', orderStrs);
                }

                result.Sql = $"SELECT {result.Columns} from {result.Table} {joinStr} {where} {orderStr} {ProcessPagedSql(result.Page, result.Count)} ";
                if (!Check.IfSqlFragmentSafe(result.Sql))
                {
                    result.Success = false;
                    result.Msg = "不安全的SQL";
                }
                results.Add(result);
            }// for table
        }

        return results;
    }

    public string ProcessPagedSql(in int page, in int count)
    {
        int p = page < 1 ? 1 : page;
        int c = count < 1 ? 1 : count;
        return $" limit {c} offset {(p - 1) * c} ";
    }
    /// <summary>
    /// 处理子表sql
    /// </summary>
    /// <param name="childResult"></param>
    public void ProcessResultChildren(EasyApiParseResult childResult, EasyApi easy, MaFrameworkOptions frameworkOption, List<JsonObject> parentListData = null)
    {
        ChildrenQueryConfig config = childResult.ChildrenConfig;
        if (childResult == null || !Check.HasValue(config.ChildField) || !Check.HasValue(config.ParentField))
        {
            return;
        }
        if (parentListData == null || parentListData.Count < 1)
        {
            return;
        }
        string sql = string.Empty;
        string parentIdFieldName = config.ParentField ?? "Id";
        if (childResult.Page > 0 && childResult.Count > 0)
        {
            foreach (var parent in parentListData)
            {
                // 分页查询
                sql = childResult.Sql.Replace("@" + config.ParentField, parent[config.ParentField]?.ToString());
                if (Check.HasValue(sql))
                {
                    var data = _dbHelper.Connection.ExecuteReader(sql);
                    var childList = easy.HandleDataReader(data, frameworkOption?.EasyApi?.ColumnAlias, childResult.Table).ToArray();
                    parent[config.KeyName ?? "children"] = new JsonArray(childList);
                }
            }
        }
        else
        {
            // 不分页，直接查询全部子项
            int whereIndex = childResult.Sql.IndexOf("WHERE");
            sql = childResult.Sql.Substring(0, whereIndex);
            if (parentListData.Any(x => x.ContainsKey(parentIdFieldName)))
            {
                var ids = parentListData.Select(x => x[parentIdFieldName]?.ToString()).ToSplitableString();
                sql += $" WHERE {config.ChildField} IN ({ids})";
            }
            var data = _dbHelper.Connection.ExecuteReader(sql);
            var childList = easy.HandleDataReader(data, frameworkOption?.EasyApi?.ColumnAlias, childResult.Table);
            if (childList.Count > 0)
            {
                foreach (var parent in parentListData)
                {
                    if (parent.ContainsKey(parentIdFieldName))
                    {
                        var childs = childList.Where(x => x[config.ChildField].ToString() == parent[parentIdFieldName].ToString()).ToArray();
                        parent[config.KeyName ?? "children"] = new JsonArray(childs);
                    }
                }
            }
        }
        return;
    }
    private EasyApiParseResult ProcessChildrenQuery(JsonNode? childBody, string table)
    {
        EasyApiParseResult result = new EasyApiParseResult()
        {
            OperationType = SqlOperationType.Table,
            Target = childBody?["target"]?.ToString()
        };

        string? target = GetTableAlias(childBody?["target"]?.ToString());
        string targetField = childBody?["targetField"]?.ToString();
        string parentField = childBody?["parentField"]?.ToString();
        string pageStr = childBody?["page"]?.ToString();
        string countStr = childBody?["count"]?.ToString();
        string customResultFieldName = childBody?["customResultFieldName"]?.ToString();
        string columns = childBody?["columns"]?.ToString() ?? "*";

        if (columns.Trim() != "*" && !columns.Contains(targetField))
        {
            columns += $",{targetField}";
        }

        int page = 1;
        int count = 20;
        int.TryParse(pageStr, out page);
        int.TryParse(countStr, out count);
        result.Page = page;
        result.Count = count;

        var child = childBody?["children"];
        if (child != null)
        {
            result.Children = new();
            var childResult = ProcessChildrenQuery(child, target);
            if (childResult != null)
            {
                result.Children = childResult;
            }
        }
        string originalColumn = string.Join(',', GetOriginalColumn(columns));
        result.Sql = $"SELECT {originalColumn} FROM {target} WHERE {targetField} = @{parentField} {ProcessPagedSql(page, count)} ";
        result.ChildrenConfig = new ChildrenQueryConfig()
        {
            KeyName = customResultFieldName,
            ChildField = targetField,
            ParentField = parentField
        };
        result.Table = target;
        return result;
    }

    private string ProcessWhereParam(EasyApiParseResult result, JsonNode node)
    {
        List<string> tmps = new();

        foreach (var whereField in node.AsObject())
        {
            // @and  @or
            string key = whereField.Key;
            if (key.Equals("@and", StringComparison.CurrentCultureIgnoreCase))
            {
                tmps.Add(ProcessWhereAndOr(whereField.Value, result.Table, "AND"));
            }
            if (key.Equals("@or", StringComparison.CurrentCultureIgnoreCase))
            {
                tmps.Add(ProcessWhereAndOr(whereField.Value, result.Table, "OR"));
            }

            string fieldName = MakeSureHavaTablePrefix(whereField.Key, result.Table);
            if (whereField.Value == null)
            {
                continue;
            }
            var fieldType = whereField.Value?.AsObject()["type"]?.ToString();
            var fieldValue = whereField.Value?.AsObject()["value"]?.ToString();
            if (Check.HasValue(fieldValue))
            {
                tmps.Add(GetOperatorNotationByWhereType(fieldType, _maFrameworkOptions.DBType, fieldName, fieldValue));
            }
        }
        return tmps.Count > 0 ? " WHERE " + string.Join(" AND ", tmps) : string.Empty;
    }

    private string ProcessWhereAndOr(JsonNode? value, string table, string andOr)
    {
        if (value == null)
        {
            return string.Empty;
        }
        List<string> items = new List<string>();
        var obj = value.AsObject();
        foreach (var field in obj)
        {
            if (field.Value == null)
            {
                continue;
            }
            if (field.Key.Equals("@and", StringComparison.CurrentCultureIgnoreCase))
            {
                items.Add(ProcessWhereAndOr(field.Value, table, "AND"));
            }
            if (field.Key.Equals("@or", StringComparison.CurrentCultureIgnoreCase))
            {
                items.Add(ProcessWhereAndOr(field.Value, table, "OR"));
            }
            string fieldName = MakeSureHavaTablePrefix(field.Key, table);

            var fieldType = field.Value?.AsObject()["type"]?.ToString();
            var fieldValue = field.Value?.AsObject()["value"]?.ToString();
            if (Check.HasValue(fieldValue))
            {
                items.Add(GetOperatorNotationByWhereType(fieldType, _maFrameworkOptions.DBType, fieldName, fieldValue));
            }
        }
        return items.Count > 0 ? "(" + string.Join(" " + andOr + " ", items) + ")" : string.Empty;
    }


    /// <summary>
    /// 确保字段名前带上表名前缀 TABLE.COLUMN
    /// </summary>
    /// <param name="fieldName"></param>
    /// <param name="table"></param>
    /// <returns></returns>
    private string MakeSureHavaTablePrefix(string fieldName, string table)
    {
        if (fieldName.Contains('.'))
        {
            var fieldNameAray = fieldName.Split('.');
            if (fieldNameAray.Length == 3)
            {
                string alias = GetTableAlias(fieldNameAray[0]);
                return alias + "." + fieldNameAray[2];
            }
            return fieldName;
        }
        else
        {
            return Check.HasValue(table) ? table + "." + fieldName : fieldName;
        }
    }
    private void ProcessOrderByPart(KeyValuePair<string, JsonNode?> subProperty, List<string> orderStrs, string table)
    {
        if (subProperty.Value == null)
        {
            return;
        }
        foreach (var item in subProperty.Value.AsArray())
        {
            var obj = item.AsObject();
            string? field = obj["field"]?.ToString();
            string? type = obj["type"]?.ToString();

            if (!Check.HasValue(field))
            {
                continue;
            }
            field = MakeSureHavaTablePrefix(field, table);
            if (!Check.HasValue(type))
            {
                type = "ASC";
            }

            field = GetFieldAlias(field);
            orderStrs.Add($"{field} {type}");
        }
    }

    private void ProcessJoinPart(KeyValuePair<string, JsonNode?> subProperty, List<string> joinStrs, string table)
    {
        if (subProperty.Value == null)
        {
            return;
        }
        foreach (var item in subProperty.Value.AsArray())
        {
            var obj = item.AsObject();
            string? targetJoin = obj["targetJoin"]?.ToString();
            string? targetOn = obj["targetOn"]?.ToString();
            string? joinField = obj["joinField"]?.ToString();
            string? onField = obj["onField"]?.ToString();
            string? joinType = obj["type"]?.ToString();

            if (!Check.HasValue(targetJoin) || !Check.HasValue(joinField) || !Check.HasValue(onField)) // || !Check.HasValue(targetField) )
            {
                continue;
            }
            if (!Check.HasValue(joinType))
            {
                joinType = "join";
            }
            targetJoin = GetTableAlias(targetJoin);
            targetOn = GetTableAlias(targetOn);
            joinType = GetJoinType(joinType);
            joinField = GetFieldAlias(joinField);
            onField = GetFieldAlias(onField);
            string result = string.Empty;
            if (!Check.HasValue(targetOn))
            {
                result = $" {joinType} {targetJoin} ON {targetJoin}.{joinField} = {table}.{onField} ";
            }
            else
            {
                result = $" {joinType} {targetJoin} ON {targetJoin}.{joinField} = {targetOn}.{onField} ";
            }

            joinStrs.Add(result);
        }
    }

    /// <summary>
    /// 获取单个字段的别名
    /// </summary>
    /// <param name="name"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    private string GetFieldAlias(string name)
    {
        if (_maFrameworkOptions.EasyApi?.ColumnAlias == null)
        {
            return name;
        }

        if (_maFrameworkOptions.EasyApi?.ColumnAlias?.ContainsKey(name) == true)
        {
            return _maFrameworkOptions.EasyApi.ColumnAlias[name];
        }
        return name;
    }

    private string GetJoinType(string joinType)
    {
        switch (joinType.ToLower())
        {
            case ConstStrings.JoinType.InnerJoin:
                return "INNER JOIN";
            case ConstStrings.JoinType.OuterJoin:
                return "OUTER JOIN";
            case ConstStrings.JoinType.LeftJoin:
                return "LEFT JOIN";
            case ConstStrings.JoinType.RightJoin:
                return "RIGHT JOIN";
            default:
                return "JOIN";
        }
    }

    /// <summary>
    /// 新增数据
    /// </summary>
    /// <param name="tableNode"></param>
    /// <returns></returns>
    private EasyApiParseResult ProcessAddData(KeyValuePair<string, JsonNode?> tableNode, EasyApiOptions easyApiOptions, bool isMulti = false)
    {
        EasyApiParseResult result = new EasyApiParseResult()
        {
            OperationType = SqlOperationType.None
        };
        if (tableNode.Value == null)
        {
            return result;
        }



        void HandleValue(JsonNode? node, List<string> cols, List<dynamic> vals, List<string> valuesParts)
        {
            if (node == null)
            {
                return;
            }
            // 获取主键，自增忽略，非自增自动生成默认值
            (string k, dynamic v) primaryKey = GetPrimaryKeyDefaultValueOfTable(tableNode.Key);
            foreach (var item in node.AsObject())
            {
                cols.Add(item.Key);
                if (long.TryParse(item.Value.ToString(), out long v))
                {
                    vals.Add(v);
                }
                else
                {
                    vals.Add("'" + item.Value.ToString() + "'");
                }
            }
            if (!cols.Contains(primaryKey.k))
            {
                // 未包含主键
                cols.Add(primaryKey.k);
                vals.Add("'" + primaryKey.v + "'");
            }
            valuesParts.Add("(" + string.Join(',', vals) + ")");
        }

        string target = tableNode.Key;
        string tableName = GetTableAlias(target);
        List<string> cols = new();
        JsonArray bulkItemArray = new();
        List<string> valuesParts = new();
        List<dynamic> vals = new();
        if (isMulti)
        {
            bulkItemArray = tableNode.Value.AsArray();
        }
        else
        {
            HandleValue(tableNode.Value.AsObject(), cols, vals, valuesParts);
        }
        foreach (var bulkItem in bulkItemArray)
        {
            cols.Clear();
            vals.Clear();
            HandleValue(bulkItem, cols, vals, valuesParts);
        }
        string originalColumn = string.Join(',', GetOriginalColumn(cols));
        result.Sql = $"INSERT INTO {tableName} ({originalColumn}) VALUES {string.Join(',', valuesParts)}";
        if (!Check.IfSqlFragmentSafe(result.Sql))
        {
            result.Success = false;
            result.Msg = "不安全的SQL";
        }
        return result;
    }
    /// <summary>
    /// 获取表的主键
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    private (string, dynamic?) GetPrimaryKeyDefaultValueOfTable(string table)
    {
        var types = _cacheManager.Get("TableColType") as Dictionary<string, IEnumerable<TableColType>>;
        if (types == null)
        {
            types = new Dictionary<string, IEnumerable<TableColType>>();
        }
        if (!types.ContainsKey(table))
        {
            // 请求
            string sql = $@"
           SELECT
            COLUMN_NAME 'Name',
            COLUMN_TYPE 'TypeLength',
            IF(EXTRA='auto_increment',CONCAT(COLUMN_KEY,'(', IF(EXTRA='auto_increment','自增长',EXTRA),')'),COLUMN_KEY) 'Key',
            IS_NULLABLE 'Isnull',
            COLUMN_COMMENT 'Remark'
            FROM
                information_schema.COLUMNS
            WHERE  TABLE_NAME = '{table}';
            ";
            var tmp = _dbHelper.Connection.Query<TableColType>(sql);
            types.Add(table, tmp);
            _cacheManager.Save("TableColType", types);
        }

        var list = types[table];
        var primaryKey = list.FirstOrDefault(x => x.Key.Contains("PRI"));
        if (primaryKey == null)
        {
            return (string.Empty, null);
        }
        return (primaryKey.Name, GetDBTypeDefaultValue(primaryKey.TypeLength, table, primaryKey.Name));
    }

    /// <summary>
    /// 获取数据库类型的默认值
    /// </summary>
    /// <param name="typeLength"></param>
    /// <returns></returns>
    private dynamic? GetDBTypeDefaultValue(string typeLength, string table, string fieldName)
    {
        if (!Check.HasValue(typeLength))
        {
            return null;
        }
        if ("char(36)".Equals(typeLength, StringComparison.CurrentCultureIgnoreCase))
        {
            return Guid.NewGuid().ToString();
        }

        if (typeLength.ToLower().Contains("int")
            || typeLength.ToLower().Contains("double")
            || typeLength.ToLower().Contains("decimal")
            || typeLength.ToLower().Contains("long")
            || typeLength.ToLower().Contains("numeric")
            || typeLength.ToLower().Contains("float"))
        {
            // 获取数据中当前最大值
            dynamic? val = GetCurrentMaxValue(table, fieldName);
            return val + 1;
        }

        return null;
    }

    private dynamic? GetCurrentMaxValue(string table, string fieldName)
    {
        if (!Check.HasValue(table) || !Check.HasValue(fieldName))
        {
            return null;
        }
        return _dbHelper.Connection.ExecuteScalar($"SELECT MAX({fieldName}) FROM {table}");
    }

    private string GetOperatorNotationByWhereType(string type, DBType dbType, string left, string right)
    {
        string result = string.Empty;
        switch (type)
        {
            case ConstStrings.WhereConfitionType.Contains:
                result = $"{left} LIKE '%{right}%'";
                break;
            case ConstStrings.WhereConfitionType.NotEqual:
                result = $"{left} != {GetRightPart(right)}";
                break;
            case ConstStrings.WhereConfitionType.In:
                result = $"{left} IN {GetRightPart(right)}";
                break;
            case ConstStrings.WhereConfitionType.LessThan:
                result = $"{left} < {GetRightPart(right)}";
                break;
            case ConstStrings.WhereConfitionType.GreaterThan:
                result = $"{left} > {GetRightPart(right)}";
                break;
            case ConstStrings.WhereConfitionType.LessThanOrEqual:
                result = $"{left} <= {GetRightPart(right)}";
                break;
            case ConstStrings.WhereConfitionType.GreaterThanOrEqual:
                result = $"{left} >= {GetRightPart(right)}";
                break;
            case ConstStrings.WhereConfitionType.Equal:
            default:
                result = $"{left}={GetRightPart(right)}";
                break;
        }

        string GetRightPart(string right)
        {
            List<string> tmp = new();
            var cols = right.Split(',');
            foreach (var item in cols)
            {
                if (int.TryParse(item, out int v))
                {
                    tmp.Add(item);
                }
                else
                {
                    tmp.Add("'" + item + "'");
                }
            }

            return string.Join(',', tmp);
        }

        return result;
    }

    public async Task<List<EasyApiParseResult>> ProcessPutRequest(Stream body, MaFrameworkOptions value)
    {
        List<EasyApiParseResult> results = new();


        var root = await JsonNode.ParseAsync(body);
        if (root == null)
        {
            return results;
        }

        var objArray = root.AsArray();
        foreach (var obj in objArray)
        {
            EasyApiParseResult result = new EasyApiParseResult()
            {
                OperationType = SqlOperationType.None
            };
            if (obj == null)
            {
                continue;
            }
            var id = string.Empty;// obj["@id"]?.ToString();
            var ids = string.Empty;// obj["@ids"]?.ToString();
            JsonNode? where = null;// obj["@where"];

            var target = string.Empty;//GetTableAlias(obj["@target"]?.ToString());

            List<string> setList = new();
            foreach (var prop in obj.AsObject())
            {
                if (prop.Key.Equals("@id", StringComparison.CurrentCultureIgnoreCase))
                {
                    id = obj["@id"]?.ToString();
                    continue;
                }
                if (prop.Key.Equals("@ids", StringComparison.CurrentCultureIgnoreCase))
                {
                    ids = obj["@ids"]?.ToString();
                    continue;
                }
                if (prop.Key.Equals("@where", StringComparison.CurrentCultureIgnoreCase))
                {
                    where = obj["@where"];
                    continue;
                }
                if (prop.Key.Equals("@target", StringComparison.CurrentCultureIgnoreCase))
                {
                    target = GetTableAlias(obj["@target"]?.ToString());
                    continue;
                }
                var key = GetFieldAlias(prop.Key);
                var val = prop.Value;
                setList.Add($"{key}='{val}'");
            }
            if (!Check.HasValue(target))
            {
                continue;
            }
            if (!Check.HasValue(id) && !Check.HasValue(ids) && where == null)
            {
                continue;
            }
            if (setList.Count > 0)
            {
                result.Sql = $"UPDATE {target} SET {string.Join(',', setList)}";
                if (Check.HasValue(id))
                {
                    result.Sql += $" WHERE ID='{id}'";
                }
                else if (Check.HasValue(ids))
                {
                    var idsTmp = ids.Split(',').ToStringCollection();
                    result.Sql += $" WHERE ID IN ({string.Join(',', idsTmp)})";
                }
                else if (where != null)
                {
                    result.Sql += ProcessWhereParam(result, where);
                }
            }
            if (!Check.IfSqlFragmentSafe(result.Sql))
            {
                result.Success = false;
                result.Msg = "不安全的SQL";
            }
            results.Add(result);
        }

        return results;
    }
}



public class EasyApiParseResult()
{
    public int Page { get; set; }
    public int Count { get; set; }
    public string Table { get; set; }
    public string Target { get; set; }
    public int? Total { get; set; }
    public string Sql { get; set; }
    public string TotalSql { get; set; }
    public string Columns { get; set; }
    public bool Success { get; set; } = true;
    public string Msg { get; set; } = string.Empty;
    public SqlOperationType OperationType { get; set; }
    /// <summary>
    /// 嵌套查询
    /// </summary>
    public EasyApiParseResult? Children { get; set; }
    /// <summary>
    /// 嵌套查询的子对象查询配置
    /// </summary>
    public ChildrenQueryConfig? ChildrenConfig { get; set; }
    /// <summary>
    /// 子表查询的数据
    /// </summary>
}

public class ChildrenQueryConfig
{
    public string KeyName { get; set; }
    public string ChildField { get; set; }
    public string ParentField { get; set; }
    // public List<JsonObject>? ChildData { get; set; }

}

public class WhereField
{
    /// <summary>
    /// ConstStrings.WhereConfitionType
    /// </summary>
    public string? Type { get; set; }
    public object Value { get; set; }
}

public class JoinField
{
    /// <summary>
    /// 连表对象
    /// </summary>
    public string TargetJon { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string TargetOn { get; set; }
    /// <summary>
    /// 连表方式
    /// </summary>
    public string Type { get; set; }
    /// <summary>
    /// ConstStrings.JoinType
    /// </summary>
    public string TargetField { get; set; }
    /// <summary>
    /// 连表字段
    /// </summary>
    public string Field { get; set; }

}

public enum SqlOperationType
{
    None = 0,
    Scalar = 1,
    Row = 2,
    Table = 3,
    /// <summary>
    /// 嵌套查询
    /// </summary>
    NestTable = 4,
}

public class TableColType
{
    public string Name { get; set; }
    public string TypeLength { get; set; }
    public string Key { get; set; }
    public string Isnull { get; set; }
    public string Remark { get; set; }
}