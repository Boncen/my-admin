using MyAdmin.Core.Repository;

namespace MyAdmin.ApiHost.db;

public class LogRepository : RepositoryBase<Log, Guid, MaDbContext>, ILogRepository
{
    public LogRepository(IServiceProvider serviceProvider) : base(serviceProvider)
    {   
    }
}