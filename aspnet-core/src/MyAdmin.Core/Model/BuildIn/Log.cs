using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Extensions.Logging;
using MyAdmin.Core.Entity;

namespace MyAdmin.Core.Model.BuildIn;

[Table("Log")]
public class Log : Entity<Guid>
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