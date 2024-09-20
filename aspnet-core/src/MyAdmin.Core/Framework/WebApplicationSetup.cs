using System.Data;
using System.Text.Json.Nodes;
using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MyAdmin.Core.Exception;
using MyAdmin.Core.Framework.Middlewares;
using MyAdmin.Core.Model;
using MyAdmin.Core.Options;
using MyAdmin.Core.Repository;
using MySql.Data.MySqlClient;

namespace MyAdmin.Core.Framework;

public static class WebApplicationSetup
{
    public static void SetupSwaggerUi(this WebApplication app, ConfigurationManager configuration)
    {
        var useVersioningStr = configuration[$"{nameof(Conf.Setting.MaFrameworkOptions)}:{nameof(Conf.Setting.MaFrameworkOptions.UseApiVersioning)}"];
        if (string.IsNullOrEmpty(useVersioningStr) || !bool.TryParse(useVersioningStr, out bool useApiVersioning) || !useApiVersioning)
        {
            app.UseSwaggerUI();
            return;
        }

        app.UseSwaggerUI(x =>
        {
            foreach (var desc in app.DescribeApiVersions())
            {
                x.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json", desc.GroupName);
            }
        });
    }
    public static void UseMaFramework(this WebApplication app, ConfigurationManager configurationManager)
    {
        var config = new MaFrameworkOptions();
        var frameworkOptions = configurationManager.GetSection("MaFrameworkOptions");
        frameworkOptions.Bind(config);
        // app.UseHttpsRedirection();
        if (config.UseGlobalErrorHandler == true)
        {
            app.UseMiddleware<ErrorHandlerMiddleware>();
        }

        if (config.UseJwtBearer == true)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        if (config.UseRateLimit == true)
        {
            app.UseRateLimiter();
            app.MapControllers().RequireRateLimiting(Conf.ConstStrings.RateLimitingPolicyName);
        }
        else
        {
            app.MapControllers();
        }

        // swagger
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.SetupSwaggerUi(configurationManager);
        }

        if (config.UseRequestLog == true)
        {
            app.UseMiddleware<RequestMonitorMiddleware>();
        }
        app.UseEasyApi();
    }

    public static void UseEasyApi(this WebApplication app, string url = "/easy")
    {
        // 简单的查询，分页查询
        app.MapGet((url), async ([FromServices] IHttpContextAccessor accessor, [FromServices] DBHelper dbHelper,
            [FromServices] IOptionsSnapshot<MaFrameworkOptions> frameworkOption, [FromServices] EasyApi easy) =>
        {
            try
            {
                var queryCollection = accessor.HttpContext.Request.Query;
                EasyApiOptions? easyApiOptions = frameworkOption.Value?.EasyApi ?? new();

                var parseResult = easy.ProcessQueryRequest(queryCollection);
                if (parseResult.Success == false)
                {
                    return ApiResult.Fail(parseResult.Msg);
                }
                if (!Check.HasValue(parseResult.Sql))
                {
                    return ApiResult.Ok();
                }

                var data = await dbHelper.Connection.ExecuteReaderAsync(parseResult.Sql);
                var result = easy.HandleDataReader(data, easyApiOptions.ColumnAlias, parseResult.Table);
                if (parseResult.Page == 1 && parseResult.Count == 1)
                {
                    return ApiResult<dynamic>.Ok(result.FirstOrDefault());
                }
                if (Check.HasValue(parseResult.TotalSql))
                {
                    var total = dbHelper.Connection.ExecuteScalar<int>(parseResult.TotalSql);
                    // return ApiResult<dynamic>.Ok(PageResult<dynamic>.Ok(result, total));
                    return ApiResult<dynamic>.Ok(new { list = result, total });
                }
                return ApiResult<dynamic>.Ok(result);
            }
            catch (MySqlException e)
            {
                throw new MAException("SQL异常", e);
            }
        });

        // 新增数据，复杂查询
        app.MapPost((url), async ([FromServices] IHttpContextAccessor accessor, [FromServices] DBHelper dbHelper,
            [FromServices] IOptionsSnapshot<MaFrameworkOptions> frameworkOption, [FromServices] EasyApi easy) =>
        {
            try
            {
                var body = accessor.HttpContext.Request.Body;
                var parseResults = await easy.ProcessPostRequest(body, frameworkOption.Value);
                JsonObject jobj = new JsonObject(); // table node
                int index = 0;
                foreach (var parseResult in parseResults)
                {
                    ++index;
                    if (parseResult.Success == false)
                    {
                        if (Check.HasValue(parseResult.Target))
                        {
                            jobj[parseResult.Target] = parseResult.Msg ?? "解析失败";
                        }
                        else{
                            jobj["target" + index] = parseResult.Msg ?? "解析失败";
                        }
                        continue;
                    }
                    if (Check.HasValue(parseResult.Sql))
                    {
                        if (parseResult.OperationType == SqlOperationType.None)
                        {
                            int affectRows = await dbHelper.Connection.ExecuteAsync(parseResult.Sql);
                            jobj["rows"] = affectRows;
                        }
                        else
                        {
                            var data = await dbHelper.Connection.ExecuteReaderAsync(parseResult.Sql);
                            var result = easy.HandleDataReader(data, frameworkOption.Value?.EasyApi?.ColumnAlias, parseResult.Table);
                            if (parseResult.Page == 1 && parseResult.Count == 1)
                            {
                                jobj.Add(parseResult.Target, result.FirstOrDefault());
                            }
                            else
                            {
                                JsonObject pageResultJobj = new JsonObject();
                                if (Check.HasValue(parseResult.TotalSql))
                                {
                                    var total = dbHelper.Connection.ExecuteScalar<int>(parseResult.TotalSql);
                                    pageResultJobj["total"] = total;
                                }
                                pageResultJobj["list"] = new JsonArray(result.ToArray());
                                jobj.Add(parseResult.Target, pageResultJobj);
                            }
                        }
                    }

                }
                return ApiResult<dynamic>.Ok(jobj);
            }
            catch (MySqlException e)
            {
                throw new MAException("SQL异常", e);
            }
        });

        app.MapPut((url), async ([FromServices] IHttpContextAccessor accessor, [FromServices] DBHelper dbHelper,
             [FromServices] IOptionsSnapshot<MaFrameworkOptions> frameworkOption, [FromServices] EasyApi easy) =>
         {
             IDbTransaction trans = null;
             try
             {
                 var body = accessor.HttpContext.Request.Body;
                 List<EasyApiParseResult> parseResults = await easy.ProcessPutRequest(body, frameworkOption.Value);
                 JsonObject jobj = new JsonObject();
                 int rows = 0;
                 dbHelper.Connection.Open();
                 trans = dbHelper.Connection.BeginTransaction();
                 foreach (var parseResult in parseResults)
                 {
                     if (!Check.HasValue(parseResult.Sql) || parseResult.Success == false)
                     {
                         return ApiResult<dynamic>.Fail(parseResult.Msg ?? "解析错误");
                     }
                     rows += await dbHelper.Connection.ExecuteAsync(parseResult.Sql);
                     jobj["rows"] = rows;
                 }
                 trans.Commit();

                 return ApiResult<dynamic>.Ok(jobj);
             }
             catch (MySqlException e)
             {
                 trans?.Rollback();
                 throw new MAException("SQL异常", e);
             }
         });

        app.MapDelete((url), async ([FromServices] IHttpContextAccessor accessor, [FromServices] DBHelper dbHelper,
            [FromServices] IOptionsSnapshot<MaFrameworkOptions> frameworkOption, [FromServices] EasyApi easy) =>
        {
            try
            {
                var queryCollection = accessor.HttpContext.Request.Query;
                EasyApiOptions? easyApiOptions = frameworkOption.Value?.EasyApi ?? new();

                var parseResult = easy.ProcessDeleteRequest(queryCollection);
                if (parseResult.Success == false)
                {
                    return ApiResult.Fail(parseResult.Msg);
                }
                if (!Check.HasValue(parseResult.Sql))
                {
                    return ApiResult.Ok();
                }
                JsonObject jobj = new JsonObject();
                int affectRows = await dbHelper.Connection.ExecuteAsync(parseResult.Sql);
                jobj["rows"] = affectRows;

                return ApiResult<dynamic>.Ok(jobj);
            }
            catch (MySqlException e)
            {
                throw new MAException("SQL异常", e);
            }
        });
    }
}