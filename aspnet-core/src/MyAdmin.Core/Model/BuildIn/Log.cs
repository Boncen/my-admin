using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyAdmin.Core.Entity;
using MyAdmin.Core.Logger;

namespace MyAdmin.Core.Model.BuildIn;

[Table("Log")]
public class Log : Entity<Guid>
{
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public LogLevel Level { get; set; }
    public DateTime LogTime { get; set; }
    public string? Host { get; set; }
    public string? UserAgent { get; set; }
    public string? ContentType { get; set; }
    public string? Origin { get; set; }
    public string? Referer { get; set; }
    public string? Url { get; set; }
    public string? RequestBody { get; set; }
    public string? ResponseStatusCode { get; set; }
    public string? ResponseBody { get; set; }

    public string? HttpMethod { get; set; }
    public string? IpAddress { get; set; }
    public string? Exceptions { get; set; }
    public string? Content { get; set; }
    
    public LogType Type { get; set; }
}
