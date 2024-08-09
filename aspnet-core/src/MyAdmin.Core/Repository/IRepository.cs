using System.Linq.Expressions;
using MyAdmin.Core.Entity;

namespace MyAdmin.Core.Repository;
public interface IRepository
{
    public bool? IsChangeTrackingEnabled { get; protected set; }
}
public interface IRepository<TEntity> : IRepository where TEntity : class, IEntity
{
    Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);
    Task InsertManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default);
    Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);
    Task UpdateManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default);
    Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);
    Task DeleteManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default);

    Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> queryPredicate,string? sortField = null, SortOrder sortOrder = SortOrder.Unspecified,params Expression<Func<TEntity, dynamic>>[] eagerLoadingProperties);
    Task<List<TEntity>> GetPagedListAsync<TSortKey>(
        Expression<Func<TEntity, bool>> queryPredicate, Expression<Func<TEntity, TSortKey>>? sortPredicate, SortOrder sortOrder, int pageNumber, int pageSize,
        params Expression<Func<TEntity, dynamic>>[] eagerLoadingProperties);
}

public interface IRepository<TEntity, TKey> : IRepository<TEntity> where TEntity : class, IEntity
{
    Task DeleteAsync(TKey id, bool autoSave = false, CancellationToken cancellationToken = default);
    Task DeleteManyAsync(IEnumerable<TKey> ids, bool autoSave = false, CancellationToken cancellationToken = default);
    Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default, params Expression<Func<TEntity, dynamic>>[] eagerLoadingProperties);
    Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default, params Expression<Func<TEntity, dynamic>>[] eagerLoadingProperties);
}