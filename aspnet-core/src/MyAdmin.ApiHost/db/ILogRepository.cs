using MyAdmin.Core.Repository;

namespace MyAdmin.ApiHost.db;

public interface ILogRepository:IRepository<Log, Guid>
{
    
}