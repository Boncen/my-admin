using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyAdmin.Core.Entity;

namespace MyAdmin.Core.Repository;
public interface IRepository
{
    public bool? IsChangeTrackingEnabled { get; set; }
}
public interface IRepository<TEntity> : IRepository where TEntity : class, IEntity
{
    DbContext GetDbContext();
    DbSet<TEntity> GetDbSet();
    IQueryable<TEntity> GetQueryable();
    Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);
    Task InsertManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default);
    Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);
    Task UpdateManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default);
    Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);
    Task DeleteManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default);

    Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> queryPredicate,string? sortField = null, SortOrder sortOrder = SortOrder.Unspecified,params Expression<Func<TEntity, dynamic>>[] eagerLoadingProperties);
    Task<List<TEntity>> GetPagedListAsync(
        Expression<Func<TEntity, bool>> queryPredicate, Expression<Func<TEntity, dynamic>>? sortPredicate, SortOrder sortOrder, int pageNumber, int pageSize,
        params Expression<Func<TEntity, dynamic>>[] eagerLoadingProperties);

    Task<(List<TEntity>, int)> GetPagedListWithTotalAsync(
        Expression<Func<TEntity, bool>> queryPredicate, Expression<Func<TEntity, dynamic>>? sortPredicate, SortOrder sortOrder, int pageNumber, int pageSize,
        params Expression<Func<TEntity, dynamic>>[] eagerLoadingProperties);

    Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> queryPredicate,
        CancellationToken cancellationToken = default, params Expression<Func<TEntity, dynamic>>[] eagerLoadingProperties);
}

public interface IRepository<TEntity, TKey> : IRepository<TEntity> where TEntity : class, IEntity
{
    Task DeleteAsync(TKey id, bool autoSave = false, CancellationToken cancellationToken = default);
    Task DeleteManyAsync(IEnumerable<TKey> ids, bool autoSave = false, CancellationToken cancellationToken = default);
    Task<TEntity> GetByIdAsync(TKey id, CancellationToken cancellationToken = default, params Expression<Func<TEntity, dynamic>>[] eagerLoadingProperties);
    Task<TEntity?> FindByIdAsync(TKey id, CancellationToken cancellationToken = default, params Expression<Func<TEntity, dynamic>>[] eagerLoadingProperties);
}

public interface IRepository<TEntity, TKey, TDbcontext>:IRepository<TEntity, TKey>  where TEntity : class, IEntity{}