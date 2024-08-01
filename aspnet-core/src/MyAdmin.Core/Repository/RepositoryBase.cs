
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyAdmin.Core.Entity;
using MyAdmin.Core.Entity.Auditing;

namespace MyAdmin.Core.Repository;

public class RepositoryBase(IServiceProvider serviceProvider) : IRepository
{
    protected DbContext _dbContext = serviceProvider.GetRequiredService<DbContext>();
}

public class RepositoryBase<TEntity>(IServiceProvider serviceProvider)
    : RepositoryBase(serviceProvider), IRepository<TEntity>
    where TEntity : class, IEntity
{
    protected static IQueryable<TEntity> IncludeDetails(
        IQueryable<TEntity> query,
        Expression<Func<TEntity, object>>[]? propertySelectors)
    {
        if (propertySelectors != null && propertySelectors.Length > 0)
        {
            foreach (var propertySelector in propertySelectors)
            {
                query = query.Include(propertySelector);
            }
        }

        return query;
    }
    protected static Expression<Func<TEntity, object>>? GetDefaultSortPredicate()
    {
        if (typeof(IHasCreationTime).IsAssignableFrom(typeof(TEntity)))
            return entity => ((IHasCreationTime)entity).CreationTime;
        return null;
    }
    private IQueryable<TEntity> SortByField(IQueryable<TEntity> queryable, string? sortField, SortOrder sortOrder)
    {
        if (!Check.HasValue(sortField))
        {
            // sortField = GetDefaultSortField();
        }

        return null;
    }
    public async Task DeleteAsync(TEntity entity, bool autoSave = false)
    {
        if (entity is ISoftDelete)
        {
            (entity as IDeletionAuditedObject<TEntity>).IsDeleted = true;
            (entity as IDeletionAuditedObject<TEntity>).DeletionTime = DateTime.Now;
            _dbContext.Set<TEntity>().Update(entity);
        }
        else
        {
            _dbContext.Remove(entity);
        }

        if (autoSave)
        {
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task DeleteManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            await DeleteAsync(entity, autoSave);
        }
    }

    public Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> queryPredicate, string? sortField, SortOrder sortOrder,
        params Expression<Func<TEntity, dynamic>>[] eagerLoadingProperties)
    {
        var query = _dbContext.Set<TEntity>().AsQueryable();
        query = IncludeDetails(query, eagerLoadingProperties);
        query = query.Where(queryPredicate);
        query = SortByField(query, sortField, sortOrder);

        return null;
    }

    public Task<List<TEntity>> GetPagedListAsync<TSortKey>(Expression<Func<TEntity, bool>> queryPredicate, Expression<Func<TEntity, TSortKey>>? sortPredicate, SortOrder sortOrder,
        int pageNumber, int pageSize, params Expression<Func<TEntity, dynamic>>[] eagerLoadingProperties)
    {
        throw new NotImplementedException();
    }

    public async Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        await _dbContext.AddAsync(entity, cancellationToken: cancellationToken);
        if (autoSave)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
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

public class RepositoryBase<TEntity, TKey>(IServiceProvider serviceProvider)
    : RepositoryBase<TEntity>(serviceProvider), IRepository<TEntity, TKey>
    where TEntity : class, IEntity
{
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
    protected RepositoryBase(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _dbContext = serviceProvider.GetRequiredService<TDbContext>();
    }
}