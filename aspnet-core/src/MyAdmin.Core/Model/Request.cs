namespace MyAdmin.Core.Model;

public class Request
{
    
}

public class PageRequest
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public bool? ReturnTotal { get; set; }
}