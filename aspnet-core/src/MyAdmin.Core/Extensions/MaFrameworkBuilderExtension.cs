using System.Reflection;
using Asp.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using MyAdmin.ApiHost.Swagger;
using MyAdmin.Core.Logger;
using MyAdmin.Core.Options;
using MyAdmin.Core.Repository;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MyAdmin.Core.Extensions;

public static class MaFrameworkBuilderExtension
{
    public static IServiceCollection AddMaFramework(this IServiceCollection service, ConfigurationManager configuration, Action<MaFrameworkBuilder> builderAction = null, params Assembly[] assemblies)
    {
        if (assemblies.Length == 0) 
            assemblies = new[] { Assembly.GetEntryAssembly() };
        var builder = new MaFrameworkBuilder(service, assemblies);
        builderAction?.Invoke(builder);
        service.AddSingleton(builder);
        #region repository

        service.TryAddScoped(typeof(IRepository<>), typeof(RepositoryBase<>));
        service.TryAddScoped(typeof(IRepository<,>), typeof(RepositoryBase<,>));

        #endregion
        
        #region log

        service.TryAddSingleton<MyAdmin.Core.Logger.ILogger, MyAdmin.Core.Logger.Logger>();

        #endregion

        #region options
        service.Configure<ApiVersioningConfOption>(configuration.GetSection(nameof(Core.Conf.Setting.ApiVersioning)));
        service.Configure<LoggerOption>(configuration.GetSection(nameof(Core.Conf.Setting.Logger)));
        #endregion
        return service;
        // service.Configure<ApiVersioningConfOption>(configuration.GetSection(nameof(Core.Conf.Setting.ApiVersioning)));
        // service.Configure<LoggerOption>(configuration.GetSection(nameof(Core.Conf.Setting.Logger)));
    }
    
    public static void UseApiVersioning(this MaFrameworkBuilder builder, ConfigurationManager configuration)
    {
        var useVersioningStr = configuration[$"{nameof(Core.Conf.Setting.ApiVersioning)}:{nameof(Core.Conf.Setting.ApiVersioning.UseApiVersioning)}"];
        if (string.IsNullOrEmpty(useVersioningStr) || !bool.TryParse(useVersioningStr, out bool useApiVersioning) || !useApiVersioning)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            return;
        }

        var apiVersionBuilder = builder.Services.AddApiVersioning(o =>
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
        builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        builder.Services.AddSwaggerGen(x => x.OperationFilter<SwaggerDefaultValues>());
    }
}