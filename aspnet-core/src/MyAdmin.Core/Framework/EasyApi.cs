using System.Data;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Nodes;
using System.Transactions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Options;
using MyAdmin.Core.Conf;
using MyAdmin.Core.Entity;
using MyAdmin.Core.Exception;
using MyAdmin.Core.Extensions;
using MyAdmin.Core.Options;
using MyAdmin.Core.Utilities;

namespace MyAdmin.Core.Framework;

public class EasyApi
{
    private readonly Core.MaFrameworkBuilder _maFrameworkBuilder;
    private readonly ICacheManager<List<string>> _cacheManager;
    private readonly MaFrameworkOptions _maFrameworkOptions;
    public EasyApi(Core.MaFrameworkBuilder maFrameworkBuilder, ICacheManager<List<string>> cacheManager, IOptionsSnapshot<MaFrameworkOptions> optionSnap)
    {
        _maFrameworkBuilder = maFrameworkBuilder;
        _cacheManager = cacheManager;
        _maFrameworkOptions = optionSnap.Value;
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
        result.Sql = $"SELECT {result.Columns} from {result.Table} {where} {order} limit {result.Count} offset {(result.Page - 1) * result.Count} ";

#if DEBUG
        Console.WriteLine(result.Sql);
#endif
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

    /// <summary>
    /// 列别名转为表列名
    /// </summary>
    /// <param name="columns"></param>
    /// <param name="option"></param>
    /// <returns></returns>
    private List<string> GetOriginalColumn(List<string> columns, EasyApiOptions option)
    {
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
                if (tableNode.Value == null) // user: {
                {
                    continue;
                }

                List<string> queryList = new();
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
                        // column 添加表名前缀
                        //var cols = result.Columns.Split()
                    }
                    if (subProperty.Key.Equals("@where", StringComparison.CurrentCultureIgnoreCase))
                    {
                        foreach (var whereField in subProperty.Value.AsObject())
                        {
                            string fieldName = MakeSureHavaTablePrefix(whereField.Key, result.Table);
                            if (whereField.Value == null)
                            {
                                continue;
                            }
                            var fieldType = whereField.Value?.AsObject()["type"]?.ToString();
                            var fieldValue = whereField.Value?.AsObject()["value"]?.ToString();
                            if (Check.HasValue(fieldValue))
                            {
                                queryList.Add(GetOperatorNotationByWhereType(fieldType, option.DBType, fieldName, fieldValue, result.Table));
                            }
                        }
                    }
                    if (subProperty.Key.Equals("@order", StringComparison.CurrentCultureIgnoreCase))
                    {
                        ProcessOrderByPart(subProperty, orderStrs, result.Table);
                    }
                } // for property
                  // 查询的情况
                string where = "";
                string joinStr = string.Empty;
                string orderStr = string.Empty;
                if (queryList.Count > 0)
                {
                    where += " WHERE " + string.Join(" AND ", queryList);
                }
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

                result.Sql = $"SELECT {result.Columns} from {result.Table} {joinStr} {where} {orderStr} limit {result.Count} offset {(result.Page - 1) * result.Count} ";
                results.Add(result);
            }// for table
        }

        return results;
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

            return table + "." + fieldName;
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
        string originalColumn = string.Join(',', GetOriginalColumn(cols, easyApiOptions));
        result.Sql = $"INSERT INTO {tableName} ({originalColumn}) VALUES {string.Join(',', valuesParts)}";
        return result;
    }

    private string GetOperatorNotationByWhereType(string type, DBType dbType, string left, string right, string table)
    {
        string result = string.Empty;
        if (!left.Contains("."))
        {
            left = table + "." + left;
        }
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

}
