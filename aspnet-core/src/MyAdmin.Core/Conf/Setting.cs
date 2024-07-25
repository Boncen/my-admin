using MyAdmin.Core.Logger;
using MyAdmin.Core.Options;

namespace MyAdmin.Core.Conf;

public class Setting {
    public LoggerOption? Logger { get; set; }
    public ApiVersioningConfOption? ApiVersioning { get; set; }
    
}