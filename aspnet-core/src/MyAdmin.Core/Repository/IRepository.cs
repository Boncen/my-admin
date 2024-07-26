using MyAdmin.Core.Entity;

namespace MyAdmin.Core.Repository;
public interface IRepository
{
    bool? IsChangeTrackingEnabled { get; }
}
public interface IRepository<TEntity> where TEntity : class, IEntity
{
    Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);
    Task InsertManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default);
    Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);
    Task UpdateManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default);
    Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);
    Task DeleteManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default);

    Task<List<TEntity>> GetListAsync(bool includeDetails = false, CancellationToken cancellationToken = default);
    Task<List<TEntity>> GetPagedListAsync(
        int skipCount,
        int maxResultCount,
        string sorting,
        bool includeDetails = false,
        CancellationToken cancellationToken = default);
}

public interface IRepository<TEntity, TKey> : IRepository<TEntity> where TEntity : class, IEntity
{
    Task DeleteAsync(TKey id, bool autoSave = false, CancellationToken cancellationToken = default);
    Task DeleteManyAsync(IEnumerable<TKey> ids, bool autoSave = false, CancellationToken cancellationToken = default);
    Task<TEntity> GetAsync(TKey id, bool includeDetails = true, CancellationToken cancellationToken = default);

    Task<TEntity?> FindAsync(TKey id, bool includeDetails = true, CancellationToken cancellationToken = default);
}