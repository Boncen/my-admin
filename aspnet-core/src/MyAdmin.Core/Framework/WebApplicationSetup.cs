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

    public static void UseErrorHandleMiddleware(this WebApplication app){
        app.UseMiddleware<ErrorHandlerMiddleware>();
    }

    public static void UseRequestMonitorMiddleware(this WebApplication app)
    {
        app.UseMiddleware<RequestMonitorMiddleware>();
    }
    
    public static void UseMaFramework(this WebApplication app, ConfigurationManager configurationManager)
    {
        app.UseHttpsRedirection();
        app.MapControllers();
        // swagger
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.SetupSwaggerUi(configurationManager);
        }

        var maframeworkOptions = app.Services.GetService(typeof(IOptions<MaFrameworkOptions>)) as IOptions<MaFrameworkOptions>;
        if (maframeworkOptions!=null)
        {
            if (maframeworkOptions.Value.UseRequestLog == true)
            {
                app.UseRequestMonitorMiddleware();
            }

            if (maframeworkOptions.Value.UseGlobalErrorHandler == true)
            {
                app.UseErrorHandleMiddleware();
            }
        }
       
    }
}