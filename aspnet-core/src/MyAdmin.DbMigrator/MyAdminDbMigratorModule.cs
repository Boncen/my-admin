using MyAdmin.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace MyAdmin.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(MyAdminEntityFrameworkCoreModule),
    typeof(MyAdminApplicationContractsModule)
    )]
public class MyAdminDbMigratorModule : AbpModule
{
}
