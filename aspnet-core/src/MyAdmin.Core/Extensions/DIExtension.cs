using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MyAdmin.Core.Repository;

namespace MyAdmin.Core.Extensions;

public static class DIExtension
{
    public static void UseLogger(this IServiceCollection service)
    {
        service.TryAddSingleton<MyAdmin.Core.Logger.ILogger, MyAdmin.Core.Logger.Logger>();
    }

    public static void UseEfCoreRepository(this IServiceCollection service)
    {
        service.TryAddScoped(typeof(IRepository<>), typeof(RepositoryBase<>));
        service.TryAddScoped(typeof(IRepository<,>), typeof(RepositoryBase<,>));
    }
}

public static class FrameworkExtension
{
    public static void UseMAFrameWork(this IServiceCollection service, Action<MaFrameworkBuilder> featureOption)
    {
        // setup options
        
    }
}

// public class FrameworkFeatureOption
// {
//     public void Hanl(){
//
//     } 
// }