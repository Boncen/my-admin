using System.Collections;

namespace MyAdmin.Core.Repository;

public class PagedResult<TEntity>{
    private readonly int _pageNumber;
    private readonly int _pageSize;
    private readonly int _recordCount;
    private readonly int _pageCount;
    private readonly List<TEntity> _results;

    /// <summary>
    /// 结果集合
    /// </summary>
    public List<TEntity> Results { get { return _results; } }

    /// <summary>
    ///  当前页
    /// </summary>
    public int PageNumber { get { return _pageNumber; } }
    /// <summary>
    /// 每页记录数
    /// </summary>
    public int PageSize { get { return _pageSize; } }
    /// <summary>
    /// 总记录数
    /// </summary>
    public int RecordCount { get { return _recordCount; }}
    /// <summary>
    /// 总页数
    /// </summary>
    public int PageCount { get { return _pageCount; } }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="recordCount"></param>
    /// <param name="results"></param>
    public PagedResult(int pageNumber, int pageSize, int recordCount, List<TEntity> results)
    {
        _pageNumber = pageNumber;
        _pageSize = pageSize;
        _recordCount = recordCount;
        _results = results;
        _pageCount = _recordCount%_pageSize == 0 ? _recordCount/_pageSize : (_recordCount/_pageSize + 1);
    }

 
}