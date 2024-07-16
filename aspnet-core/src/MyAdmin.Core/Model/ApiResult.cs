namespace MyAdmin.Core;

public class ApiResult
{
    public int? Code { get; set; }
    public string? Msg { get; set; }
    public dynamic? Data { get; set; }
    public bool? Success { get; set; }
}
