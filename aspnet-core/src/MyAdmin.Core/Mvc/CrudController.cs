using System.Linq.Expressions;
using System.Reflection;
using Dapper;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using MyAdmin.Core.Conf;
using MyAdmin.Core.Entity;
using MyAdmin.Core.Exception;
using MyAdmin.Core.Framework.Attribute;
using MyAdmin.Core.Model;
using MyAdmin.Core.Repository;
using Org.BouncyCastle.Bcpg;

namespace MyAdmin.Core.Mvc;

public class CrudController<TEntity, TKey, TAdd, TSearch, TResponse> : MAController where TEntity : class, IEntity
{
    private readonly IRepository<TEntity, TKey> _repository;
    public readonly DBHelper _dbHelper;

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
        var type = addInput.GetType();
        var properties = type.GetProperties();
        foreach (var property in properties)
        {
            var val = property.GetValue(addInput);
            if (val == null)
            {
                continue;
            }

            var tableFieldAttr = property.GetCustomAttribute<ValidateTableFieldAttribute>();
            if (tableFieldAttr != null)
            {
                var dbset = _repository.GetDbSet();
                var result = await _dbHelper.Connection.ExecuteScalarAsync(
                    $"select {tableFieldAttr.FieldName} from {tableFieldAttr.TableName} where {tableFieldAttr.FieldName} = '{val}'");
                if (result == null)
                {
                    throw new MAException(tableFieldAttr.ErrorMessage ?? $"{property.Name}值不存在");
                }
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
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        var rsp = entity.Adapt<TResponse>();
        return rsp;
    }

    [HttpPost]
    public virtual async Task<ApiResult<List<TResponse>>> List([FromQuery] ListRequest? request,
        [FromBody] TSearch? search)
    {
        var query = GenerateQueryPredicate(search);
        var sortType = request.Desc == true ? SortOrder.Descending :
            request.Desc == false ? SortOrder.Ascending : SortOrder.Unspecified;
        var result = await _repository.GetListAsync(query, request.SortField, sortType);
        return ApiResult<List<TResponse>>.Ok("success", result.Adapt<List<TResponse>>());
    }

    [HttpPost]
    public virtual async Task<ApiResult<PageResult<TResponse>>> PageList([FromQuery] PageRequest request,
        [FromBody] TSearch? search,
        CancellationToken cancellationToken)
    {
        List<TEntity> entities = default;
        int total = 0;
        var query = GenerateQueryPredicate(search);
        var sortType = request.Desc == true ? SortOrder.Descending :
            request.Desc == false ? SortOrder.Ascending : SortOrder.Unspecified;
        Expression<Func<TEntity, dynamic>> sortPredicate = null;
        if (Check.HasValue(request.SortField))
        {
            sortPredicate = GenerateSortPredicate(request.SortField);
        }
        if (request.ReturnTotal == true)
        {
            var entitiesWithTotal = await _repository.GetPagedListWithTotalAsync(query, sortPredicate,
                sortType,
                request.PageIndex, request.PageSize);
            entities = entitiesWithTotal.Item1;
            total = entitiesWithTotal.Item2;
        }
        else
        {
            entities = await _repository.GetPagedListAsync(query, sortPredicate, sortType,
                request.PageIndex, request.PageSize);
        }

        var rsp = entities.Adapt<List<TResponse>>();
        return ApiResult<PageResult<TResponse>>.Ok("success",
            new PageResult<TResponse>() { List = rsp, Total = total });
    }

    private Expression<Func<TEntity, dynamic>>? GenerateSortPredicate(string requestSortField)
    {
        if (!Check.HasValue(requestSortField))
        {
            return null;
        }

        var entityProp = typeof(TEntity).GetProperty(requestSortField);
        if (entityProp == null)
        {
            return null;
        }
        
        ParameterExpression paramExpr = Expression.Parameter(typeof(TEntity), "entity");
        MemberExpression propertyAccessExpr = Expression.Property(paramExpr, entityProp);
        UnaryExpression conversion = Expression.TypeAs(propertyAccessExpr, typeof(object));
        var exp = Expression.Lambda<Func<TEntity, dynamic>>(conversion,paramExpr);
        return exp;
    }

    /// <summary>
    /// 解析请求参数
    /// </summary>
    /// <param name="search"></param>
    /// <typeparam name="TSearch"></typeparam>
    /// <returns></returns>
    private Expression<Func<TEntity, bool>> GenerateQueryPredicate<TSearch>(TSearch? search)
    {
        if (search == null)
        {
            return null;
        }

        var tType = search.GetType();
        var properties = tType.GetProperties();
        if (properties.Length < 1)
        {
            return null;
        }

        ParameterExpression param = Expression.Parameter(typeof(TEntity), "entity");
        Expression<Func<TEntity, bool>> query = (entity) => true;
        var entityProperties = typeof(TEntity).GetProperties();
        // 存储所有的条件
        var conditions = new List<Expression>();
        // 创建参数表达式
        ParameterExpression paramExpr = Expression.Parameter(typeof(TEntity), "entity");
        foreach (var prop in properties)
        {
            var value = prop.GetValue(search);
            if (value == null)
            {
                continue;
            }

            var entityProp = entityProperties.FirstOrDefault(x => x.Name == prop.Name);
            if (entityProp == null)
            {
                continue;
            }

            
            // 创建常量表达式
            Expression constantExpr = Expression.Constant(value, entityProp.PropertyType);
            if (entityProp.PropertyType != prop.PropertyType)
            {
                constantExpr = Expression.Convert(constantExpr, entityProp.PropertyType);
            }

            // 创建属性访问表达式
            MemberExpression propertyAccessExpr = Expression.Property(paramExpr, entityProp);
            // 创建相等比较表达式
            BinaryExpression equalityExpr = Expression.Equal(propertyAccessExpr, constantExpr);
            // 创建Lambda表达式
            var exp = Expression.Lambda<Func<TEntity, bool>>(equalityExpr, paramExpr);
            conditions.Add(exp);
        }

        if (!conditions.Any())
        {
            return null;
        }

        Expression aggregateCondition = conditions[0];
        for (int i = 1; i < conditions.Count; i++)
        {
            aggregateCondition = Expression.AndAlso(aggregateCondition, conditions[i]);
        }

        return Expression.Lambda<Func<TEntity, bool>>(aggregateCondition, param);
    }
}