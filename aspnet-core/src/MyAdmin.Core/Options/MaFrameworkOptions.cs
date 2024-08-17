using MyAdmin.Core.Framework;

namespace MyAdmin.Core.Options;

public class MaFrameworkOptions
{
    public bool? UseGlobalErrorHandler { get; set; } = true;
    public bool? UseRequestLog { get; set; } = false;
    public bool? UseApiVersioning { get; set; } = false;
    public bool? SaveRequestBody { get; set; }
    public bool? SaveResponseBody { get; set; }
    public DBType DBType { get; set; }
    
    
}