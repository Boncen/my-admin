namespace MyAdmin.Core.Model;

public class Result
{
    public int? Status { get; set; }
}
public class ApiResult :Result
{
    public string? Title { get; set; }
}
public class Result<T> : Result
{
    public T? Data { get; set; }
}
public class ApiResult<T> : ApiResult
{
    public T Data { get; set; }
}

public class PageResult
{
    public virtual List<dynamic> List { get; set; }
    public int? Total { get; set; }
}

public class PageResult<T>:PageResult
{
    public new List<T> List { get; set; }
}