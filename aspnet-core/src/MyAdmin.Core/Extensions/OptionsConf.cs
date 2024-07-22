using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyAdmin.Core.Options;

namespace MyAdmin.Core.Extensions;

public static class OptionsConf
{
    public static void SetupOptions(this IServiceCollection service, ConfigurationManager configuration)
    {
        service.Configure<ApiVersioningConfOption>(configuration.GetSection(Core.Conf.ConstKey.ApiVersioning));
    }
}
