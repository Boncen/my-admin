namespace MyAdmin.Core.Options;

public class MaFrameworkOptions
{
    public bool? UseGlobalErrorHandler { get; set; } = true;
    public bool? UseRequestLog { get; set; } = false;
    public bool? UseApiVersioning { get; set; } = false;
}