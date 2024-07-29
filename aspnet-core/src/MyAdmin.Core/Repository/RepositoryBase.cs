
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyAdmin.Core.Entity;

namespace MyAdmin.Core.Repository;

public class RepositoryBase : IRepository
{
    protected DbContext _dbContext;
    public RepositoryBase(IServiceProvider serviceProvider)
    {
        _dbContext = serviceProvider.GetRequiredService<DbContext>();
    }
}

public class RepositoryBase<TEntity> : RepositoryBase, IRepository<TEntity> where TEntity : class, IEntity
{
    public RepositoryBase(IServiceProvider serviceProvider) : base(serviceProvider)
    {

    }
    public Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
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

    public async Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        await _dbContext.AddAsync(entity, cancellationToken: cancellationToken);
        if (autoSave)
        {
            await _dbContext.SaveChangesAsync();
        }

        return entity;
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

public class RepositoryBase<TEntity, TKey> : RepositoryBase<TEntity>, IRepository<TEntity, TKey> where TEntity : class, IEntity
{
    public RepositoryBase(IServiceProvider serviceProvider) : base(serviceProvider)
    {

    }
    public Task DeleteAsync(TKey id, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteManyAsync(IEnumerable<TKey> ids, bool autoSave = false, CancellationToken cancellationToken = default)
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
}

public class RepositoryBase<TEntity, TKey, TDbContext> : RepositoryBase<TEntity, TKey> 
    where TEntity : class, IEntity
    where TDbContext : DbContext
{
    public RepositoryBase(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _dbContext = serviceProvider.GetRequiredService<TDbContext>();
    }
}