using Volo.Abp.Modularity;

namespace MyAdmin;

public abstract class MyAdminApplicationTestBase<TStartupModule> : MyAdminTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
