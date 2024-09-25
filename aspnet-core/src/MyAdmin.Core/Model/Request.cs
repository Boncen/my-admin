using System.Globalization;

namespace MyAdmin.Core.Model;

public class Request
{
    public string? Keyword { get; set; }
}

public class ListRequest : Request
{
    private string? _sortField;

    /// <summary>
    ///     排序字段
    /// </summary>
    public string? SortField
    {
        get => Check.HasValue(_sortField) ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_sortField) : string.Empty;
        set => _sortField = value;
    }

    /// <summary>
    ///     是否降序
    /// </summary>
    public bool? Desc { get; set; }
}

public class PageRequest : ListRequest
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }

    /// <summary>
    ///     是否在分页接口中返回全部数量
    /// </summary>
    public bool? ReturnTotal { get; set; } = true;
}

public enum FieldRequestType
{
    Equal = 1,
    GreaterThan,
    LessThan,
    GreaterOrEqual,
    LessOrEqual,
    Contain
}

public class RequestField
{
    public FieldRequestType Type { get; set; }
    public dynamic Value { get; set; }
}