using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MyAdmin.Core.Extensions;

public static class DIExtension
{
    public static void UseLogger(this IServiceCollection service)
    {
        service.TryAddSingleton<MyAdmin.Core.Logger.ILogger, MyAdmin.Core.Logger.Logger>();
    }
}
