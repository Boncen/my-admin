namespace MyAdmin.ApiHost.Dto;

public class LogDto
{
    public Guid? UserId { get; set; }
    public LogLevel Level { get; set; }
    public string? UserName { get; set; }
    public DateTime LogTime { get; set; }
    public string? IpAddress { get; set; }
    public string? BrowserInfo { get; set; }
    public string? HttpMethod { get; set; }
    public string? Exceptions { get; set; }
    public string? Content { get; set; }
}

public class AddLogDto
{
    public string? UserName { get; set; }
    public string? Content { get; set; }
}