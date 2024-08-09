using Microsoft.EntityFrameworkCore;
using MyAdmin.Core.Entity;

namespace MyAdmin.Core.Extensions;
public static class EfCoreRepositoryExtensions
{
    public static IQueryable<TEntity> AsNoTrackingIf<TEntity>(this IQueryable<TEntity> queryable, bool condition)
            where TEntity : class, IEntity
    {
        return condition ? queryable.AsNoTracking() : queryable;
    }
}