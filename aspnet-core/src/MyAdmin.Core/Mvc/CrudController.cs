using System.Reflection;
using Dapper;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAdmin.Core.Entity;
using MyAdmin.Core.Exception;
using MyAdmin.Core.Framework.Attribute;
using MyAdmin.Core.Model;
using MyAdmin.Core.Repository;

namespace MyAdmin.Core.Mvc;

public class CrudController<TEntity, TKey, TAdd, TResponse> : MAController where TEntity : class, IEntity
{
    private readonly IRepository<TEntity, TKey> _repository;
    public readonly DBHelper _dbHelper;

    public CrudController(IRepository<TEntity, TKey> repository, DBHelper dbHelper)
    {
        _repository = repository;
        _dbHelper = dbHelper;
    }

    [HttpPost]
    public async Task<TResponse> Add([FromBody] TAdd addInput, CancellationToken cancellationToken)
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
    public async Task Delete(TKey id, CancellationToken cancellationToken)
    {
        await _repository.DeleteAsync(id, true, cancellationToken);
    }

    [HttpDelete]
    public async Task DeleteRange(TKey[] ids, CancellationToken cancellationToken)
    {
        await _repository.DeleteManyAsync(ids, true, cancellationToken);
    }

    [HttpPut]
    public async Task Update([FromBody] TEntity entity, CancellationToken cancellationToken)
    {
        await _repository.UpdateAsync(entity, true, cancellationToken);
    }

    [HttpPut]
    public async Task UpdateMany([FromBody] TEntity[] entities, CancellationToken cancellationToken)
    {
        await _repository.UpdateManyAsync(entities, true, cancellationToken);
    }

    [HttpGet]
    public async Task<TResponse> GetOne(TKey id, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetAsync(id, cancellationToken);
        var rsp = entity.Adapt<TResponse>();
        return rsp;
    }

    [HttpGet]
    public async Task<ApiResult<PageResult<TResponse>>> PageList([FromQuery] PageRequest request,
        CancellationToken cancellationToken)
    {
        List<TEntity> entities = default;
        int total = 0;
        if (request.ReturnTotal == true)
        {
            var entitiesWithTotal = await _repository.GetPagedListWithTotalAsync<TEntity>(null, null,
                SortOrder.Unspecified,
                request.PageIndex, request.PageSize);
            entities = entitiesWithTotal.Item1;
            total = entitiesWithTotal.Item2;
        }
        else
        {
            entities = await _repository.GetPagedListAsync<TEntity>(null, null, SortOrder.Unspecified,
                request.PageIndex, request.PageSize);
        }

        var rsp = entities.Adapt<List<TResponse>>();
        ApiResult<PageResult<TResponse>> res = new ApiResult<PageResult<TResponse>>()
        {
            Data = new PageResult<TResponse>() { List = rsp, Total = total },
            Status = StatusCodes.Status200OK,
        };
        return res;
    }
}