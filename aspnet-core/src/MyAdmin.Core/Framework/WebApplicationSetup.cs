using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MyAdmin.Core.Framework.Middlewares;
using MyAdmin.Core.Model;
using MyAdmin.Core.Options;
using MyAdmin.Core.Repository;

namespace MyAdmin.Core.Framework;

public static class WebApplicationSetup
{
    public static void SetupSwaggerUi(this WebApplication app, ConfigurationManager configuration)
    {
        var useVersioningStr = configuration[$"{nameof(Conf.Setting.MaFrameworkOptions)}:{nameof(Conf.Setting.MaFrameworkOptions.UseApiVersioning)}"];
        if (string.IsNullOrEmpty(useVersioningStr) || !bool.TryParse(useVersioningStr, out bool useApiVersioning ) || !useApiVersioning)
        {
            app.UseSwaggerUI();
            return;
        }

        app.UseSwaggerUI(x => {
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
            app.MapControllers().RequireRateLimiting(Conf.ConstSettingValue.RateLimitingPolicyName);
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
        app.MapGet((url), async ([FromServices]IHttpContextAccessor accessor,[FromServices]DBHelper dbHelper, 
            [FromServices]IOptionsSnapshot<MaFrameworkOptions> frameworkOption, [FromServices]EasyApi easy) =>
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
            return ApiResult<dynamic>.Ok(result);
        });
    }
}