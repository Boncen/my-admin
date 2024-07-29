using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MyAdmin.Core.Extensions;

public static class MaFrameworkBuilderExtension
{
    public static IServiceCollection AddMaFramework(this IServiceCollection service, Action<MaFrameworkBuilder> builderAction = null, params Assembly[] assemblies)
    {
        if (assemblies.Length == 0) 
            assemblies = new[] { Assembly.GetEntryAssembly() };
        var builder = new MaFrameworkBuilder(service, assemblies);
        builderAction?.Invoke(builder);
        service.AddSingleton(builder);

        return service;
        // service.Configure<ApiVersioningConfOption>(configuration.GetSection(nameof(Core.Conf.Setting.ApiVersioning)));
        // service.Configure<LoggerOption>(configuration.GetSection(nameof(Core.Conf.Setting.Logger)));
    }
    
    
}