using System.Text.Json.Nodes;
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

    public EasyApiOptions? EasyApi { get; set; }
    public CacheOptions? Cache { get; set; }
}

public class CacheOptions
{
    /// <summary>
    ///     
    /// </summary>
    public CacheTypeEnum? CacheType { get; set; }
    
    public string? RedisUrl { get; set; }
}

public enum CacheTypeEnum
{
    Memerory = 1,
    Redis
}

public class EasyApiOptions
{
    public bool? AllowAnonymous { get; set; }
    /// <summary>
    /// 角色要求
    /// </summary>
    public string? RequireRole { get; set; }
    /// <summary>
    /// 排除的表，多个表用逗号分隔
    /// </summary>
    public string? ExcludeTable { get; set; }
    /// <summary>
    /// alias: actualName
    /// </summary>
    public Dictionary<string,string>? TableAlias { get; set; }
    /// <summary>
    /// alias: actualName
    /// </summary>
    public Dictionary<string,string>? ColumnAlias { get; set; }

    
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