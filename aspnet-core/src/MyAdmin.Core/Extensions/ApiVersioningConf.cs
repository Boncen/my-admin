using Asp.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MyAdmin.ApiHost.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MyAdmin.Core.Extensions;

public static class ApiVersioningConf
{
    public static void UseApiVersioning(this IServiceCollection service, ConfigurationManager configuration)
    {
        var useVersioningStr = configuration[$"{Core.Conf.ConstKey.ApiVersioning}:{Core.Conf.ConstKey.UseApiVersioning}"];
        if (string.IsNullOrEmpty(useVersioningStr) || !bool.TryParse(useVersioningStr, out bool useApiVersioning) || !useApiVersioning)
        {
            service.AddEndpointsApiExplorer();
            service.AddSwaggerGen();
            return;
        }

        var apiVersionBuilder = service.AddApiVersioning(o =>
        {
            o.AssumeDefaultVersionWhenUnspecified = true; // 指定默认的版本
            o.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
            o.ReportApiVersions = true;
            o.ApiVersionReader = ApiVersionReader.Combine(
                new QueryStringApiVersionReader("ver"),
                new QueryStringApiVersionReader("api-version"),
                new HeaderApiVersionReader(["ver", "X-Version", "api-version"]),
                new MediaTypeApiVersionReader("ver"),
                new MediaTypeApiVersionReader("api-version")
            );
        });
        apiVersionBuilder.AddMvc().AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });
        // configure swagger
        service.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        service.AddSwaggerGen(x => x.OperationFilter<SwaggerDefaultValues>());
    }

}
