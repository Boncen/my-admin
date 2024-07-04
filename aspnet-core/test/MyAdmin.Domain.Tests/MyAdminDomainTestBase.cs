using Volo.Abp.Modularity;

namespace MyAdmin;

/* Inherit from this class for your domain layer tests. */
public abstract class MyAdminDomainTestBase<TStartupModule> : MyAdminTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
