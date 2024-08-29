using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using MyAdmin.Core.Conf;

namespace MyAdmin.Core.Model;

public class Result
{
    public int ErrCode { get; set; } = MaErrorCode.NonError;
    public string? Error { get; set; }
}
public class ApiResult :Result
{
    public string? Title { get; set; }

    public static ApiResult Ok(string? title)
    {
        return new ApiResult()
        {
            Title = title
        };
    }
    public static ApiResult Fail(string? title, int errCode = MaErrorCode.LogicError)
    {
        return new ApiResult()
        {
            ErrCode = errCode,
            Error = title,
        };
    }
}
// public class Result<T> : Result
// {
//     public T? Data { get; set; }
// }
public class ApiResult<T> : ApiResult
{
    public T? Data { get; set; }
    public static ApiResult<T> Ok(string? title, T? data)
    {
        return new ApiResult<T>()
        {
            Title = title,
            Data = data
        };
    }
}

public class PageResult:ApiResult
{
    public virtual List<dynamic> List { get; set; }
    public int? Total { get; set; }
}

public class PageResult<T>:PageResult
{
    public new List<T> List { get; set; }
}