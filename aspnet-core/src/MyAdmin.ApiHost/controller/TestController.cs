using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using MyAdmin.ApiHost.db;
using MyAdmin.Core.Repository;
using ILogger = MyAdmin.Core.Logger.ILogger;

namespace MyAdmin.ApiHost;

[ApiController]
[ApiVersion("1.0")]
public class TestController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IRepository<Log,Guid> _logRepository;
    public TestController(ILogger logger, IRepository<Log,Guid> logRepository)
    {
        _logger = logger;
        _logRepository = logRepository;
    }


    [HttpPost(ApiEndpoints.Test.TestAdd)]
    public Task<string> TestAdd(string content)
    {
        _logRepository.InsertAsync(new Log()
        {
            Id = Guid.NewGuid(),
            Content = content,
            LogTime = DateTime.Now
        }, true);
        return Task.FromResult("ok");
    }

    [HttpDelete(ApiEndpoints.Test.TestDelete)]
    public async Task<string> TestDelete(Guid id, CancellationToken cancellationToken)
    { 
        await _logRepository.DeleteAsync(id, true, cancellationToken);
        return "ok";
    }

    [HttpDelete("deletemany")]
    public async Task TestDeleteMany(Guid[] ids, CancellationToken cancellationToken)
    {
        await _logRepository.DeleteManyAsync(ids, true, cancellationToken);
    }
    
    [HttpPut("update")]
    public async Task TestUpdate(Guid id,string content, CancellationToken cancellationToken)
    {
        var log = await _logRepository.GetAsync(id);

        log.Content = content;
        await _logRepository.UpdateAsync(log, true, cancellationToken);
    }
    
    [HttpPut("updatemany")]
    public async Task TestUpdateMany(Guid[] ids,string content, CancellationToken cancellationToken)
    {
        var logs = await _logRepository.GetListAsync((log1 => ids.Contains(log1.Id)), string.Empty, SortOrder.Unspecified);
        foreach (var log in logs)
        {
            log.Content = content;
        }
        await _logRepository.UpdateManyAsync(logs, true, cancellationToken);
    }
    
    [HttpGet("getone")]
    public async Task<Log> TestGetOne(Guid id, CancellationToken cancellationToken)
    {
        var log = await _logRepository.GetAsync(id);

        return log;
    }
    
    [HttpGet("getlist")]
    public async Task<List<Log>> TestGetMany(string keyword, CancellationToken cancellationToken)
    {
        var logs = await _logRepository.GetListAsync((log1 => log1.Content.Contains(keyword)));
        return logs;
    }
    
    [HttpGet("getpagelist")]
    public async Task<List<Log>> TestGetPageMany(string keyword,int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        var logs = await _logRepository.GetPagedListAsync((log1 => log1.Content.Contains(keyword)), (log => log.LogTime), SortOrder.Descending, pageIndex, pageSize);
        return logs;
    }
}
