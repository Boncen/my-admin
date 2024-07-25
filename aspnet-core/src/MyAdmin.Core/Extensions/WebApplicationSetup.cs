using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using MyAdmin.Core.Middleware;

namespace MyAdmin.Core.Extensions;

public static class WebApplicationSetup
{
    public static void SetupSwaggerUI(this WebApplication app, ConfigurationManager configuration)
    {
        var useVersioningStr = configuration[$"{nameof(Core.Conf.Setting.ApiVersioning)}:{nameof(Core.Conf.Setting.ApiVersioning.UseApiVersioning)}"];
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

    public static void UseErrorHandleMiddleware(this WebApplication app){
        app.UseMiddleware<ErrorHandlerMiddleware>();
    }
}