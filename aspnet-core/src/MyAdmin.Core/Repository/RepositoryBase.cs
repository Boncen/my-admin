
using MyAdmin.Core.Entity;

namespace MyAdmin.Core.Repository;

public class Repository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class, IEntity
{
    public Repository()
    {
        
    }
    public Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(TKey id, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
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