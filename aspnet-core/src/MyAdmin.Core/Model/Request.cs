namespace MyAdmin.Core.Model;

public class Request
{
    public string? Keyword { get; set; }
}

public class ListRequest : Request
{
    /// <summary>
    /// 排序字段
    /// </summary>
    public string? SortField { get; set; }
    /// <summary>
    /// 是否降序
    /// </summary>
    public bool? Desc { get; set; }
}

public class PageRequest:ListRequest
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    /// <summary>
    /// 是否在分页接口中返回全部数量
    /// </summary>
    public bool? ReturnTotal { get; set; } = true;


}