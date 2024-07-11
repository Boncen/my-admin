using System.Threading.Tasks;

namespace MyAdmin.Data;

public interface IMyAdminDbSchemaMigrator
{
    Task MigrateAsync();
}
