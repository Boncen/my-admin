using MyAdmin.Core.Repository;

namespace MyAdmin.ApiHost.db;

public class LogRepository: EfCoreRepository<MaDbContext, Log, Guid>,ILogRepository
{
    public LogRepository(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}