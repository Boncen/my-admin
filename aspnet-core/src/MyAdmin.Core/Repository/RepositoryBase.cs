
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyAdmin.Core.Entity;
using MyAdmin.Core.Entity.Auditing;
using MyAdmin.Core.Extensions;

namespace MyAdmin.Core.Repository;

public class RepositoryBase(IServiceProvider serviceProvider) : IRepository
{
    protected DbContext _dbContext = serviceProvider.GetRequiredService<DbContext>();

    public bool? IsChangeTrackingEnabled { get; set; } = true;
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

    protected DbContext GetDbContext()
    {
        return _dbContext;
    }

    protected DbSet<TEntity> GetDbSet()
    {
        return _dbContext.Set<TEntity>();
    }
    protected IQueryable<TEntity> GetQueryable()
    {
        return GetDbSet().AsQueryable().AsNoTrackingIf(IsChangeTrackingEnabled == false);
    }

    private IQueryable<TEntity> SortByField(IQueryable<TEntity> queryable, string? sortField, SortOrder sortOrder)
    {
        if (!Check.HasValue(sortField))
        {
            // sortField = GetDefaultSortField();
            return queryable;
        }
        ParameterExpression parameterExpression = Expression.Parameter(typeof(TEntity), "x");
        MemberExpression memberExpression = Expression.Property(parameterExpression, sortField);
        Expression<Func<TEntity, object>> expression = Expression.Lambda<Func<TEntity, object>>(memberExpression, parameterExpression);
        switch (sortOrder)
        {
            case SortOrder.Ascending:
                return queryable.OrderBy(expression);
            case SortOrder.Descending:
                return queryable.OrderByDescending(expression);
            default:
                break;
        }
        return queryable;
    }
    public async Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        if (entity is ISoftDelete)
        {
            (entity as IDeletionAuditedObject<TEntity>).IsDeleted = true;
            (entity as IDeletionAuditedObject<TEntity>).DeletionTime = DateTime.Now;
            GetDbSet().Update(entity);
        }
        else
        {
            GetDbContext().Remove(entity);
        }

        if (autoSave)
        {
            await GetDbContext().SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            await DeleteAsync(entity, autoSave);
        }
    }

    public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> queryPredicate, string? sortField, SortOrder sortOrder,
        params Expression<Func<TEntity, dynamic>>[] eagerLoadingProperties)
    {
        var query = GetQueryable();
        query = IncludeDetails(query, eagerLoadingProperties);
        query = query.Where(queryPredicate);
        query = SortByField(query, sortField, sortOrder);

        return await query.ToListAsync();
    }

    public async Task<List<TEntity>> GetPagedListAsync<TSortKey>(Expression<Func<TEntity, bool>>? queryPredicate, Expression<Func<TEntity, TSortKey>>? sortPredicate, SortOrder sortOrder,
        int pageNumber, int pageSize, params Expression<Func<TEntity, dynamic>>[] eagerLoadingProperties)
    {
        var query = GetQueryable();
        query = IncludeDetails(query, eagerLoadingProperties);
        if (queryPredicate != null)
        {
            query = query.Where(queryPredicate);
        }

        if (sortPredicate != null)
        {
            switch (sortOrder)
            {
                case SortOrder.Ascending:
                    query = query.OrderBy(sortPredicate);
                    break;
                case SortOrder.Descending:
                    query = query.OrderByDescending(sortPredicate);
                    break;
                default:
                    break;
            }
        }
        return await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task<(List<TEntity>, int)> GetPagedListWithTotalAsync<TSortKey>(Expression<Func<TEntity, bool>> queryPredicate, Expression<Func<TEntity, TSortKey>>? sortPredicate,
        SortOrder sortOrder, int pageNumber, int pageSize, params Expression<Func<TEntity, dynamic>>[] eagerLoadingProperties)
    {
        var query = GetQueryable();
        query = IncludeDetails(query, eagerLoadingProperties);
        if (queryPredicate != null)
        {
            query = query.Where(queryPredicate);
        }

        var total = await query.CountAsync();
        if (sortPredicate != null)
        {
            switch (sortOrder)
            {
                case SortOrder.Ascending:
                    query = query.OrderBy(sortPredicate);
                    break;
                case SortOrder.Descending:
                    query = query.OrderByDescending(sortPredicate);
                    break;
                default:
                    break;
            }
        }
        var list = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return (list, total);
    }

    public async Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        await GetDbContext().AddAsync(entity, cancellationToken: cancellationToken);
        if (autoSave)
        {
            await GetDbContext().SaveChangesAsync(cancellationToken);
        }

        return entity;
    }

    public async Task InsertManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        await GetDbSet().AddRangeAsync(entities, cancellationToken);
        if (autoSave)
        {
            await GetDbContext().SaveChangesAsync(cancellationToken);
        }
    }


    public async Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        GetDbSet().Update(entity);
        if (autoSave)
        {
            await GetDbContext().SaveChangesAsync(cancellationToken);
        }
        return entity;
    }

    public async Task UpdateManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        GetDbContext().UpdateRange(entities);
        if (autoSave)
        {
            await GetDbContext().SaveChangesAsync(cancellationToken);
        }
    }
}

public class RepositoryBase<TEntity, TKey>(IServiceProvider serviceProvider)
    : RepositoryBase<TEntity>(serviceProvider), IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    public async Task DeleteAsync(TKey id, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        var entity = await GetAsync(id, cancellationToken);
        if (entity == null)
        {
            return;
        }
        await DeleteAsync(entity, autoSave, cancellationToken);
    }

    public async Task DeleteManyAsync(IEnumerable<TKey> ids, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        foreach (var id in ids)
        {
            await DeleteAsync(id, cancellationToken: cancellationToken);
        }

        if (autoSave)
        {
            await GetDbContext().SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default,
         params Expression<Func<TEntity, dynamic>>[] eagerLoadingProperties)
    {
        var query = GetQueryable();
        query = IncludeDetails(query, eagerLoadingProperties);
        var entity = await query.FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);
        return entity;
    }

    public async Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default,
         params Expression<Func<TEntity, dynamic>>[] eagerLoadingProperties)
    {
        var entity = await FindAsync(id, cancellationToken, eagerLoadingProperties);
        if (entity == null)
        {
            throw new EntityNotFoundException();
        }
        return entity;
    }
}

public class RepositoryBase<TEntity, TKey, TDbContext> : RepositoryBase<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TDbContext : DbContext
{
    protected RepositoryBase(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _dbContext = serviceProvider.GetRequiredService<TDbContext>();
    }
}