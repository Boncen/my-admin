using System.Data;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Nodes;
using System.Transactions;
using Microsoft.AspNetCore.Http;
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
    public EasyApi(Core.MaFrameworkBuilder maFrameworkBuilder, ICacheManager<List<string>> cacheManager)
    {
        _maFrameworkBuilder = maFrameworkBuilder;
        _cacheManager = cacheManager;
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
    public EasyApiParseResult ProcessQueryRequest(IQueryCollection queryCollection, EasyApiOptions option)
    {
        EasyApiParseResult result = new EasyApiParseResult()
        {
            Page = 1,
            Count = 1,
            Columns = "*",
            Sql = string.Empty,
            Table = string.Empty,
        };
        if (!queryCollection.ContainsKey("target"))
        {
            return result;
        }


        List<string> queryList = new();
        foreach (var q in queryCollection)
        {
            if (q.Key.Equals(nameof(result.Total), StringComparison.CurrentCultureIgnoreCase))
            {
                int.TryParse(q.Value.ToString(), out int total);
                result.Total = total;
                continue;
            }
            if (q.Key.Equals(nameof(result.Page), StringComparison.CurrentCultureIgnoreCase))
            {
                int.TryParse(q.Value.ToString(), out int page);
                result.Page = page;
                continue;
            }
            if (q.Key.Equals(nameof(result.Count), StringComparison.CurrentCultureIgnoreCase))
            {
                int.TryParse(q.Value.ToString(), out int count);
                result.Count = count;
                continue;
            }
            if (q.Key.Equals("target", StringComparison.CurrentCultureIgnoreCase))
            {
                result.Target = q.Value.ToString();
                result.Table = GetTableAlias(result.Target, option);
                // if (!IsEntity(result.Table))
                // {
                //     result.Success = false;
                //     result.Msg = "target无效";
                //     return result;
                // }

                continue;
            }

            if (q.Key.Equals(nameof(result.Columns), StringComparison.CurrentCultureIgnoreCase) && Check.HasValue(q.Value))
            {
                result.Columns = GetQueryColumnAlias(q.Value.ToString(), option);
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

        string where = "";
        if (queryList.Count > 0)
        {
            where += " WHERE " + string.Join(" AND ", queryList);
        }
        if (result.Total.HasValue)
        {
            result.TotalSql = $"SELECT COUNT(1) from {result.Table} {where}";
        }
        result.Sql = $"SELECT {result.Columns} from {result.Table} {where} limit {result.Count} offset {(result.Page - 1) * result.Count} ";

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
        //GetTableAlias(target, option);
    }

    /// <summary>
    /// 获取别名
    /// </summary>
    /// <param name="target"></param>
    /// <param name="option"></param>
    private string GetTableAlias(string target, EasyApiOptions option)
    {
        if (option.TableAlias != null)
        {
            if (option.TableAlias.ContainsKey(target))
            {
                var val = option.TableAlias[target];
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
                result.Table = GetTableAlias(result.Target, option.EasyApi);
                // if (!IsEntity(result.Table))
                // {
                //     result.Success = false;
                //     result.Msg = "target无效";
                //     results.Add(result);
                //     return results;
                // }

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
                    if (subProperty.Key.Equals("@where", StringComparison.CurrentCultureIgnoreCase))
                    {
                        foreach (var whereField in subProperty.Value.AsObject())
                        {
                            string fieldName = whereField.Key;
                            if (whereField.Value == null)
                            {
                                continue;
                            }
                            var fieldType = whereField.Value?.AsObject()["type"]?.ToString();
                            var fieldValue = whereField.Value?.AsObject()["value"]?.ToString();
                            if (Check.HasValue(fieldValue))
                            {
                                queryList.Add(GetOperatorNotationByWhereType(fieldType, option.DBType, fieldName, fieldValue));
                            }
                        }
                    }
                } // for property
                  // 查询的情况
                string where = "";
                if (queryList.Count > 0)
                {
                    where += " WHERE " + string.Join(" AND ", queryList);
                }
                if (result.Total.HasValue)
                {
                    result.TotalSql = $"SELECT COUNT(1) from {result.Table} {where}";
                }
                result.Sql = $"SELECT {result.Columns} from {result.Table} {where} limit {result.Count} offset {(result.Page - 1) * result.Count} ";

                // 新增数据的情况 todo
                // result.KeyResults.Add(tableNode.Key, sql);
                results.Add(result);
            }// for table
        }

        return results;
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
        string tableName = GetTableAlias(target, easyApiOptions);
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
    /// ConstSettingValue.WhereConfitionType
    /// </summary>
    public string? Type { get; set; }
    public object Value { get; set; }
}

public enum SqlOperationType
{
    None = 0,
    Scalar = 1,
    Row = 2,
    Table = 3,

}
