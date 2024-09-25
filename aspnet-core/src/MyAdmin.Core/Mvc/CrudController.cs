using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using Dapper;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using MyAdmin.Core.Entity;
using MyAdmin.Core.Exception;
using MyAdmin.Core.Framework.Attribute;
using MyAdmin.Core.Model;
using MyAdmin.Core.Repository;

namespace MyAdmin.Core.Mvc;

public class CrudController<TEntity, TKey, TAdd, TPageListSearch, TResponse> : MAController
    where TEntity : class, IEntity
    where TPageListSearch : PageRequest
{
    public readonly DBHelper _dbHelper;
    private readonly IRepository<TEntity, TKey> _repository;
    public CrudController(IRepository<TEntity, TKey> repository, DBHelper dbHelper)
    {
        _repository = repository;
        _dbHelper = dbHelper;
    }

    [HttpPost]
    public virtual async Task<TResponse> Add([FromBody] TAdd addInput, CancellationToken cancellationToken)
    {
        await Validate(addInput);
        var entity = addInput.Adapt<TEntity>();
        entity = await _repository.InsertAsync(entity, true, cancellationToken);
        return entity.Adapt<TResponse>();
    }

    private async Task Validate(TAdd addInput)
    {
        if (addInput == null)
        {
            return;
        }
        var type = addInput.GetType();
        var properties = type.GetProperties();
        foreach (var property in properties)
        {
            var val = property.GetValue(addInput);
            if (val == null) continue;

            var tableFieldAttr = property.GetCustomAttribute<ValidateTableFieldAttribute>();
            if (tableFieldAttr != null)
            {
                var dbset = _repository.GetDbSet();
                var result = await _dbHelper.Connection.ExecuteScalarAsync(
                    $"select {tableFieldAttr.FieldName} from {tableFieldAttr.TableName} where {tableFieldAttr.FieldName} = '{val}'");
                if (result == null) throw new MAException(tableFieldAttr.ErrorMessage ?? $"{property.Name}值不存在");
            }
        }
    }

    [HttpDelete]
    public virtual async Task Delete(TKey id, CancellationToken cancellationToken)
    {
        await _repository.DeleteAsync(id, true, cancellationToken);
    }

    [HttpDelete]
    public virtual async Task DeleteRange(TKey[] ids, CancellationToken cancellationToken)
    {
        await _repository.DeleteManyAsync(ids, true, cancellationToken);
    }

    [HttpPut]
    public virtual async Task Update([FromBody] TEntity entity, CancellationToken cancellationToken)
    {
        await _repository.UpdateAsync(entity, true, cancellationToken);
    }

    [HttpPut]
    public virtual async Task UpdateMany([FromBody] TEntity[] entities, CancellationToken cancellationToken)
    {
        await _repository.UpdateManyAsync(entities, true, cancellationToken);
    }

    [HttpGet]
    public virtual async Task<TResponse> GetOne(TKey id, CancellationToken cancellationToken)
    {
        _repository.IsChangeTrackingEnabled = false;
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        var rsp = entity.Adapt<TResponse>();
        return rsp;
    }

    [HttpPost]
    public virtual async Task<ApiResult<List<TResponse>>> List([FromBody] TPageListSearch? search)
    {
        _repository.IsChangeTrackingEnabled = false;
        var query = GenerateQueryPredicate(search);
        var sortType = search?.Desc == true ? SortOrder.Descending :
            search?.Desc == false ? SortOrder.Ascending : SortOrder.Unspecified;
        var result = await _repository.GetListAsync(query, search?.SortField, sortType);
        return ApiResult<List<TResponse>>.Ok(result.Adapt<List<TResponse>>());
    }

    [HttpPost]
    public virtual async Task<PageResult<TResponse>> PageList([FromBody] TPageListSearch? search,
        CancellationToken cancellationToken)
    {
        _repository.IsChangeTrackingEnabled = false;
        List<TEntity>? entities = default;
        var total = 0;
        var query = GenerateQueryPredicate(search);
        var sortType = search?.Desc == true ? SortOrder.Descending :
            search?.Desc == false ? SortOrder.Ascending : SortOrder.Unspecified;
        Expression<Func<TEntity, dynamic>>? sortPredicate = null;
        if (Check.HasValue(search?.SortField)) sortPredicate = GenerateSortPredicate(search?.SortField);
        if (search?.ReturnTotal == true)
        {
            var entitiesWithTotal = await _repository.GetPagedListWithTotalAsync(query, sortPredicate,
                sortType,
                search?.PageIndex ?? 1, search?.PageSize ?? 10);
            entities = entitiesWithTotal.Item1;
            total = entitiesWithTotal.Item2;
        }
        else
        {
            entities = await _repository.GetPagedListAsync(query, sortPredicate, sortType,
                search?.PageIndex ?? 1, search?.PageSize ?? 10);
        }
        var rsp = entities.Adapt<List<TResponse>>();
        return PageResult<TResponse>.Ok(rsp, total);
    }

    private Expression<Func<TEntity, dynamic>>? GenerateSortPredicate(string? requestSortField)
    {
        if (!Check.HasValue(requestSortField)) return null;

        var entityProp = typeof(TEntity).GetProperty(requestSortField!);
        if (entityProp == null) return null;

        var paramExpr = Expression.Parameter(typeof(TEntity), "entity");
        var propertyAccessExpr = Expression.Property(paramExpr, entityProp);
        var conversion = Expression.TypeAs(propertyAccessExpr, typeof(object));
        var exp = Expression.Lambda<Func<TEntity, dynamic>>(conversion, paramExpr);
        return exp;
    }

    /// <summary>
    ///     解析请求参数
    /// </summary>
    /// <param name="search"></param>
    /// <typeparam name="TSearch"></typeparam>
    /// <returns></returns>
    private Expression<Func<TEntity, bool>>? GenerateQueryPredicate<TSearch>(TSearch? search)
    {
        if (search == null) return null;

        var searchType = search.GetType();
        var searchProperties = searchType.GetProperties();
        if (searchProperties.Length < 1) return null;

        var param = Expression.Parameter(typeof(TEntity), "entity");
        var entityProperties = typeof(TEntity).GetProperties();
        var conditions = new List<Expression>();
        foreach (var prop in searchProperties)
        {
            var value = prop.GetValue(search);
            if (value == null) continue;

            var entityProp = entityProperties.FirstOrDefault(x => x.Name == prop.Name);
            if (entityProp == null) continue;

            // 属性
            var propertyAccessExpr = Expression.Property(param, entityProp);
            if (value is RequestField field)
            {
                var reqType = field.Type;
                var fieldValue = field.Value;
                // if (fieldValue == null)
                // {
                //     continue;
                // }
                Expression constantExpr = Expression.Constant(fieldValue?.ToString(), typeof(object));
                if (entityProp.PropertyType != field.Value?.GetType())
                    constantExpr = Expression.Convert(constantExpr, entityProp.PropertyType);
                
                switch (reqType)
                {
                    case FieldRequestType.Contain:
                        var methodCallExpr = Expression.Call(
                            propertyAccessExpr,
                            typeof(string).GetMethod("Contains", new[] { typeof(string) })!,
                            constantExpr
                        );
                        conditions.Add(methodCallExpr);
                        break;
                    case FieldRequestType.Equal:
                        conditions.Add(Expression.Equal(propertyAccessExpr, constantExpr));
                        break;
                    case FieldRequestType.GreaterThan:
                        conditions.Add(Expression.GreaterThan(propertyAccessExpr, constantExpr));
                        break;
                    case FieldRequestType.LessThan:
                        conditions.Add(Expression.LessThan(propertyAccessExpr, constantExpr));
                        break;
                    case FieldRequestType.GreaterOrEqual:
                        conditions.Add(Expression.GreaterThanOrEqual(propertyAccessExpr, constantExpr));
                        break;
                    case FieldRequestType.LessOrEqual:
                        conditions.Add(Expression.LessThanOrEqual(propertyAccessExpr, constantExpr));
                        break;
                }
            }
            else
            {
                Expression constantExpr = Expression.Constant(value, entityProp.PropertyType);
                if (entityProp.PropertyType != prop.PropertyType)
                    constantExpr = Expression.Convert(constantExpr, entityProp.PropertyType);
                // the default option is equal
                conditions.Add(Expression.Equal(propertyAccessExpr, constantExpr));
            }
        }

        if (!conditions.Any()) return null;

        var aggregateCondition = conditions[0];
        for (var i = 1; i < conditions.Count; i++)
            aggregateCondition = Expression.And(aggregateCondition, conditions[i]);

        return Expression.Lambda<Func<TEntity, bool>>(aggregateCondition, param);
    }
}