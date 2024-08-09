namespace MyAdmin.Core.Model;

public class ApiResultBase
{
    public int? Code { get; set; }
    public bool? Success { get; set; }
}
public class ApiResult :ApiResultBase
{
    
    public string? Msg { get; set; }
    
}
public class ApiResultWithData : ApiResultBase
{
    public dynamic? Data { get; set; }
}
public class ApiResultFull : ApiResult
{
    public dynamic? Data { get; set; }
}
