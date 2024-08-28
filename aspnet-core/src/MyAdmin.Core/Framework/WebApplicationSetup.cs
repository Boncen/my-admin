using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MyAdmin.Core.Framework.Middlewares;
using MyAdmin.Core.Options;

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
        var maframeworkOptions = new MaFrameworkOptions();
        var frameworkOptions = configurationManager.GetSection("MaFrameworkOptions");
        frameworkOptions.Bind(maframeworkOptions);
        // app.UseHttpsRedirection();
        if (maframeworkOptions.UseGlobalErrorHandler == true)
        {
            app.UseMiddleware<ErrorHandlerMiddleware>();
        }
        if (maframeworkOptions.UseRateLimit == true)
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
        
        if (maframeworkOptions.UseRequestLog == true)
        {
            app.UseMiddleware<RequestMonitorMiddleware>();
        }
    }
}