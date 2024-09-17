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

                var parseResult = easy.ProcessQueryRequest(queryCollection, easyApiOptions);
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
                throw new MAException("查询中发生错误", e);
            }
        });

        // 新增数据，复杂查询
        app.MapPost((url), async ([FromServices] IHttpContextAccessor accessor, [FromServices] DBHelper dbHelper,
            [FromServices] IOptionsSnapshot<MaFrameworkOptions> frameworkOption, [FromServices] EasyApi easy) =>
        {
            try
            {
                var body = accessor.HttpContext.Request.Body;
                // EasyApiOptions? easyApiOptions = frameworkOption.Value?.EasyApi ?? new();

                var parseResult = await easy.ProcessPostRequest(body, frameworkOption.Value);
                if (parseResult.Success == false)
                {
                    return ApiResult.Fail(parseResult.Msg);
                }
                if (Check.HasValue(parseResult.Sql))
                {
                    var data = await dbHelper.Connection.ExecuteReaderAsync(parseResult.Sql);
                    var result = easy.HandleDataReader(data, frameworkOption.Value?.EasyApi?.ColumnAlias, parseResult.Table);
                    if (parseResult.Page == 1 && parseResult.Count == 1)
                    {
                        return ApiResult<dynamic>.Ok(result.FirstOrDefault());
                    }
                    if (Check.HasValue(parseResult.TotalSql))
                    {
                        var total = dbHelper.Connection.ExecuteScalar<int>(parseResult.TotalSql);
                        return ApiResult<dynamic>.Ok(new { list = result, total });
                    }
                }

                if (parseResult.KeyResults.Count > 0)
                {
                    JsonObject jobj = new JsonObject();
                    foreach (var item in parseResult.KeyResults)
                    {
                        var data = await dbHelper.Connection.ExecuteReaderAsync(item.Value as string);
                        var result = easy.HandleDataReader(data, frameworkOption.Value?.EasyApi?.ColumnAlias, parseResult.Table);
                        if (parseResult.Page == 1 && parseResult.Count == 1)
                        {
                            jobj.Add(item.Key, result.FirstOrDefault());
                            // return ApiResult<dynamic>.Ok(result.FirstOrDefault());
                        }
                        if (Check.HasValue(parseResult.TotalSql))
                        {
                            var total = dbHelper.Connection.ExecuteScalar<int>(parseResult.TotalSql);
                            var tmpListRes = new JsonObject();
                            // tmpListRes.Add(item.Key, );
                            // return ApiResult<dynamic>.Ok(new { list = result, total });
                        }
                    }
                }

                return ApiResult<dynamic>.Ok();
            }
            catch (MySqlException e)
            {
                throw new MAException("查询中发生错误", e);
            }
        });

        app.MapPut((url), async () =>
        {

        });

        app.MapDelete((url), async () =>
        {

        });
    }
}