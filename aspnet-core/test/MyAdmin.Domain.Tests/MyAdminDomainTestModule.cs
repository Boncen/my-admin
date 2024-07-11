using Volo.Abp.Modularity;

namespace MyAdmin;

[DependsOn(
    typeof(MyAdminDomainModule),
    typeof(MyAdminTestBaseModule)
)]
public class MyAdminDomainTestModule : AbpModule
{

}
