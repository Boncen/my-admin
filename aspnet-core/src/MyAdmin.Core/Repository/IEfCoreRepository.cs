using Microsoft.EntityFrameworkCore;
using MyAdmin.Core.Entity;

namespace MyAdmin.Core.Repository;

public interface IEfCoreRepository<TEntity> : IRepository<TEntity>
    where TEntity : class, IEntity
{
    Task<DbContext> GetDbContextAsync();

    Task<DbSet<TEntity>> GetDbSetAsync();
}

public interface IEfCoreRepository<TEntity, TKey> : IEfCoreRepository<TEntity>, IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{

}
