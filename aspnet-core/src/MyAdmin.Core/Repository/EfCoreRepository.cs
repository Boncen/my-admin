using Microsoft.EntityFrameworkCore;
using MyAdmin.Core.Entity;

namespace MyAdmin.Core.Repository;

public abstract class EfCoreRepository<TDbContext, TEntity> : IEfCoreRepository<TEntity> where TEntity : class, IEntity where TDbContext : DbContext
{

    //  protected virtual Task<TDbContext> GetDbContextAsync()
    // {
    //     // Multi-tenancy unaware entities should always use the host connection string
    //     if (!EntityHelper.IsMultiTenant<TEntity>())
    //     {
    //         using (CurrentTenant.Change(null))
    //         {
    //             return _dbContextProvider.GetDbContextAsync();
    //         }
    //     }

    //     return _dbContextProvider.GetDbContextAsync();
    // }
    public Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<DbContext> GetDbContextAsync()
    {
        throw new NotImplementedException();
    }

    public Task<DbSet<TEntity>> GetDbSetAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<TEntity>> GetListAsync(bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<TEntity>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task InsertManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

public abstract class EfCoreRepository<TDbContext, TEntity, TKey> : IEfCoreRepository<TEntity, TKey> 
    where TEntity : class, IEntity<TKey> 
    where TDbContext : DbContext
{
    public Task DeleteAsync(TKey id, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteManyAsync(IEnumerable<TKey> ids, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TEntity?> FindAsync(TKey id, bool includeDetails = true, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TEntity> GetAsync(TKey id, bool includeDetails = true, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<DbContext> GetDbContextAsync()
    {
        throw new NotImplementedException();
    }

    public Task<DbSet<TEntity>> GetDbSetAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<TEntity>> GetListAsync(bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<TEntity>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task InsertManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}