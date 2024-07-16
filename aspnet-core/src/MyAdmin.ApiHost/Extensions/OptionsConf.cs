using MyAdmin.ApiHost.Options;

namespace MyAdmin.ApiHost.Extensions;

public static class OptionsConf
{
    public static void SetupOptions(this IServiceCollection service, ConfigurationManager configuration)
    {
        service.Configure<ApiVersioningConfOption>(configuration.GetSection(Core.Conf.ConstKey.ApiVersioning));
    }
}
