using System.Text;
using System.Text.Json.Nodes;
using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyAdmin.Core.Model;
using MyAdmin.Core.Repository;

namespace MyAdmin.Core.EasyApi;

public static class EasyApiSetup
{
    public static void UseEasyApi(this WebApplication app, string url = "/easy")
    {
        app.MapPost(url, async ([FromServices]IHttpContextAccessor accessor, CancellationToken cancellationToken) =>
        {
            var body = accessor.HttpContext.Request.Body;
            string sql = await ProcessRequest(body, cancellationToken);
            
            return sql;
        }).WithName("easy");

        app.MapGet((url), async ([FromServices]IHttpContextAccessor accessor,[FromServices]DBHelper dbHelper, CancellationToken cancellationToken) =>
        {
            var queryCollection = accessor.HttpContext.Request.Query;
            var sql = ProcessQueryRequest(queryCollection, cancellationToken);
            if (!Check.HasValue(sql))
            {
                return ApiResult.Ok();
            }
            var data = dbHelper.Connection.Query(sql);
            return ApiResult<dynamic>.Ok(data);
        });
    }
    private static string ProcessQueryRequest(IQueryCollection queryCollection, CancellationToken cancellationToken)
    {
        int page = 1;
        int count = 1; // 默认查询单条
        string columns = "*";
        string target = String.Empty;
        if (!queryCollection.ContainsKey(nameof(target)))
        {
            return String.Empty;
        }
        
        List<string> queryList = new();
        foreach (var q in queryCollection)
        {
            if (q.Key.Equals(nameof(page), StringComparison.CurrentCultureIgnoreCase))
            {
                int.TryParse(q.Value.ToString(), out page);
                continue;
            }
            if (q.Key.Equals(nameof(count), StringComparison.CurrentCultureIgnoreCase))
            {
                int.TryParse(q.Value.ToString(), out count);
                continue;
            }
            if (q.Key.Equals(nameof(target), StringComparison.CurrentCultureIgnoreCase))
            {
                target = q.Value.ToString();
                continue;
            }

            if (q.Key.Equals(nameof(columns), StringComparison.CurrentCultureIgnoreCase) && Check.HasValue(q.Value))
            {
                columns = q.Value.ToString();
                continue;
            }
            // todo 列别名配置
            
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
        string sql = $"SELECT {columns} from {target} {where} limit {count} offset {(page - 1) * count} ";
#if DEBUG
        Console.WriteLine(sql);
#endif
        return sql;
    }
    
    private async static Task<string> ProcessRequest(Stream body, CancellationToken cancellationToken)
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
                    //  todo 判断是否存在别名
                    
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