using Mapster;
using Microsoft.AspNetCore.Mvc;
using MyAdmin.Core.Entity;
using MyAdmin.Core.Repository;

namespace MyAdmin.Core.Mvc;

public class CrudController<TEntity, TKey, TAdd, TResponse>: ControllerBase where TEntity: class, IEntity
{
    private readonly IRepository<TEntity,TKey> _repository;
    public CrudController(IRepository<TEntity,TKey> repository)
    {
        _repository = repository;
    }
    
    [HttpPost()]
    public async Task<TResponse> Add([FromBody]TAdd addInput, CancellationToken cancellationToken)
    {
        var entity = addInput.Adapt<TEntity>();
        entity = await _repository.InsertAsync(entity, true, cancellationToken);
        return entity.Adapt<TResponse>();
    }
    
    [HttpDelete()]
    public async Task Delete(TKey id, CancellationToken cancellationToken)
    {
        await _repository.DeleteAsync(id, true, cancellationToken);
    }
    
    [HttpDelete()]
    public async Task DeleteRange(TKey[] ids, CancellationToken cancellationToken)
    {
        await _repository.DeleteManyAsync(ids, true, cancellationToken);
    }
    
    [HttpPut()]
    public async Task Update([FromBody]TEntity entity, CancellationToken cancellationToken)
    {
        await _repository.UpdateAsync(entity, true, cancellationToken);
    }
    
    [HttpPut()]
    public async Task UpdateRange([FromBody]TEntity[] entities, CancellationToken cancellationToken)
    {
        await _repository.UpdateManyAsync(entities, true, cancellationToken);
    }
    
    [HttpGet()]
    public async Task<TResponse> GetOne(TKey id, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetAsync(id, cancellationToken);
        var rsp = entity.Adapt<TResponse>();
        return rsp;
    }
    
    // [HttpGet($"{nameof(TEntity)}s")]
    // public async Task<TResponse> GetAll(TKey id, CancellationToken cancellationToken)
    // {
    //     var entity = await _repository.GetListAsync(id, cancellationToken);
    //     var rsp = entity.Adapt<TResponse>();
    //     return rsp;
    // }
}