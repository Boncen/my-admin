using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace MyAdmin.Data;

/* This is used if database provider does't define
 * IMyAdminDbSchemaMigrator implementation.
 */
public class NullMyAdminDbSchemaMigrator : IMyAdminDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
