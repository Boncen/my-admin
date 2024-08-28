
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyAdmin.Core.Entity;
using MyAdmin.Core.Entity.Auditing;
using MyAdmin.Core.Exception;
using MyAdmin.Core.Extensions;

namespace MyAdmin.Core.Repository;

public class RepositoryBase() : IRepository
{
    public bool? IsChangeTrackingEnabled { get; set; } = true;
}

public class RepositoryBase<TEntity>(IServiceProvider serviceProvider)
    : RepositoryBase(), IRepository<TEntity>
    where TEntity : class, IEntity
{
    protected DbContext _dbContext = serviceProvider.GetRequiredKeyedService<DbContext>(nameof(MaDbContext));

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

    public DbContext GetDbContext()
    {
        return _dbContext;
    }

    public DbSet<TEntity> GetDbSet()
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
        if (entity is ISoftDelete || entity is IHasDeletionTime)
        {
            if (entity is ISoftDelete softDelEntity)
            {
                softDelEntity.IsDeleted = true;
            }
            if (entity is IHasDeletionTime hasDelTimeEntity )
            {
                hasDelTimeEntity.DeletionTime = DateTime.Now;
            }
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

    public Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> queryPredicate, CancellationToken cancellationToken = default)
    {
        // todo
        throw new NotImplementedException();
    }

    public async Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        if (entity is IHasCreationTime i)
        {
            if (i.CreationTime.IsDefaultValue())
            {
                i.CreationTime = DateTime.Now;
            }
        }
        await GetDbContext().AddAsync(entity, cancellationToken: cancellationToken);
        if (autoSave)
        {
            await GetDbContext().SaveChangesAsync(cancellationToken);
        }

        return entity;
    }

    public async Task InsertManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            if (entity is IHasCreationTime)
            {
                if ((entity as IHasCreationTime).CreationTime.IsDefaultValue())
                {
                    (entity as IHasCreationTime).CreationTime = DateTime.Now;
                }
            }
        }
        await GetDbSet().AddRangeAsync(entities, cancellationToken);
        if (autoSave)
        {
            await GetDbContext().SaveChangesAsync(cancellationToken);
        }
    }


    public async Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        if (entity is IHasModificationTime u)
        {
            if (!u.LastModificationTime.HasValue)
            {
                u.LastModificationTime = DateTime.Now;
            }
        }
        GetDbSet().Update(entity);
        if (autoSave)
        {
            await GetDbContext().SaveChangesAsync(cancellationToken);
        }
        return entity;
    }

    public async Task UpdateManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            if (entity is IHasModificationTime)
            {
                if (!(entity as IHasModificationTime).LastModificationTime.HasValue)
                {
                    (entity as IHasModificationTime).LastModificationTime = DateTime.Now;
                }
            }
        }
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
        var entity = await GetByIdAsync(id, cancellationToken);
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

    public async Task<TEntity?> FindByIdAsync(TKey id, CancellationToken cancellationToken = default,
         params Expression<Func<TEntity, dynamic>>[] eagerLoadingProperties)
    {
        var query = GetQueryable();
        query = IncludeDetails(query, eagerLoadingProperties);
        var entity = await query.FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);
        return entity;
    }

    public async Task<TEntity> GetByIdAsync(TKey id, CancellationToken cancellationToken = default,
         params Expression<Func<TEntity, dynamic>>[] eagerLoadingProperties)
    {
        var entity = await FindByIdAsync(id, cancellationToken, eagerLoadingProperties);
        if (entity == null)
        {
            throw new EntityNotFoundException();
        }
        return entity;
    }
}

public class RepositoryBase<TEntity, TKey, TDbContext> : RepositoryBase<TEntity, TKey>, IRepository<TEntity, TKey, TDbContext>
    where TEntity : class, IEntity<TKey>
    where TDbContext : DbContext
{
    public RepositoryBase(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _dbContext = serviceProvider.GetRequiredService<TDbContext>();
    }
}