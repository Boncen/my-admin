using System.Threading.RateLimiting;
using MyAdmin.Core.Framework;

namespace MyAdmin.Core.Options;

public class MaFrameworkOptions
{
    public bool? UseGlobalErrorHandler { get; set; } = true;
    public bool? UseRequestLog { get; set; } = false;
    public bool? UseApiVersioning { get; set; } = false;
    // public bool? UseBuildInDbContext { get; set; }
    public bool? SaveRequestBody { get; set; }
    public bool? SaveResponseBody { get; set; }
    public bool? UseJwtBearer { get; set; }
    public DBType DBType { get; set; }
    public string? DBVersion { get; set; }
    public bool? UseBuildInDbContext { get; set; } = true;
    public bool? UseRateLimit { get; set; } = false;
    public MaRateLimitOptions? RateLimitOptions { get; set; } = new();
}

public class MaRateLimitOptions
{
    public int PermitLimit { get; set; } = 50;
    /// <summary>
    /// 窗口时间长度(秒)
    /// </summary>
    public int Window { get; set; } = 60;

    public int SegmentsPerWindow { get; set; } = 5;
    // public QueueProcessingOrder QueueProcessingOrder { get; set; } = QueueProcessingOrder.OldestFirst;
    public int QueueLimit { get; set; } = 10;
}