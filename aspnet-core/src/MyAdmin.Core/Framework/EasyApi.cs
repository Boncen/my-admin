using System.Data;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;
using MyAdmin.Core.Conf;
using MyAdmin.Core.Entity;
using MyAdmin.Core.Exception;
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
        var list = _cacheManager.Get(ConstSettingValue.MACacheKeyAllEntities);
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

            _cacheManager.Save(ConstSettingValue.MACacheKeyAllEntities, list);
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
            Table = string.Empty
        };
        if (!queryCollection.ContainsKey("target"))
        {
            return result;
        }

        
        List<string> queryList = new();
        foreach (var q in queryCollection)
        {
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
                result.Table = q.Value.ToString();
                result.Table = GetTableAlias(result.Table, option);
                if (!IsEntity(result.Table))
                {
                    result.Success = false;
                    result.Msg = "target无效";
                    return result;
                }

                continue;
            }

            if (q.Key.Equals(nameof(result.Columns), StringComparison.CurrentCultureIgnoreCase) && Check.HasValue(q.Value))
            {
                result.Columns = HandleColumnAlias(q.Value.ToString(), option);
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
    private string HandleColumnAlias(string columns, EasyApiOptions option)
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

            if (entityFullName.ToLower().IndexOf( ("." + target).ToLower()) > -1)
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
                if (columnAlias.ContainsValue(name) || columnAlias.ContainsValue(table+"." + name))
                {
                    name  = columnAlias.FirstOrDefault(x =>
                        x.Value.Equals(table+"." + name) || x.Value.Equals(name, StringComparison.CurrentCultureIgnoreCase)).Key;
                    // key前面加下划线 _ 的配置项视为忽略输出
                    if (name == "_"+dtColumn.ColumnName)
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
    private async Task<string> ProcessRequest(Stream body, CancellationToken cancellationToken)
    {
        var root = await JsonNode.ParseAsync(body, cancellationToken: cancellationToken);
        if (root == null)
        {
            return "";
        }

        StringBuilder sb = new();
        var rootObject = root.AsObject();
        if (rootObject != null)
        {
            foreach (var tableNode in rootObject)
            {
                if (tableNode.Value != null)
                {
                    var tableName = tableNode.Key;
                    string columns = string.Empty;
                    int count = 0;
                    int page = 0;
                    JsonNode where = null;
                    // JsonArray join = null;
                    // JsonNode children = null;   
                    JsonObject subObject = tableNode.Value.AsObject();
                    
                    
                    foreach (var subProperty in subObject)
                    {
                        if (subProperty.Key == "@columns")
                        {
                            columns = subProperty.Value.ToString();
                        }

                        if (subProperty.Key == "@count")
                        {
                            int.TryParse(subProperty.Value.ToString(), out count);
                        }
                        
                        if (subProperty.Key == "@page")
                        {
                            int.TryParse(subProperty.Value.ToString(), out page);
                        }
                        if (subProperty.Key == "@where")
                        {
                            
                        }
                    }
                }
            }
        }

        return "";
    }
}

public class EasyApiParseResult()
{
    public int Page { get; set; }
    public int Count { get; set; }
    public string Table { get; set; }
    public string Sql { get; set; }
    public string Columns { get; set; }
    public bool Success { get; set; } = true;
    public string Msg { get; set; } = string.Empty;
}