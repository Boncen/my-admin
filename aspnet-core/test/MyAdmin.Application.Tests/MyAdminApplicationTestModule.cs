using Volo.Abp.Modularity;

namespace MyAdmin;

[DependsOn(
    typeof(MyAdminApplicationModule),
    typeof(MyAdminDomainTestModule)
)]
public class MyAdminApplicationTestModule : AbpModule
{

}
