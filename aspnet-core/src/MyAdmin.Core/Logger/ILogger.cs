using Microsoft.Extensions.Logging;
using MyAdmin.Core.Model.BuildIn;

namespace MyAdmin.Core.Logger;

public interface ILogger
{
    void Log(Log log);
    void Log(LogLevel level, string content, System.Exception? exception);
    void LogInformation(string content);
    void LogWarning(string content);
    void LogTrace(string content);
    void LogDebug(string content);
    void LogError(string content, System.Exception? exception);
    void LogError(System.Exception exception);
    void LogCritical(string content);
}