using Microsoft.Extensions.Logging;

namespace MyAdmin.Core.Logger;

public interface ILogger{
    void Log(LogLevel level, string content, Exception? exception);
    void LogInformation(string content);
    void LogWarning(string content);
    void LogTrace(string content);
    void LogDebug(string content);
    void LogError(string content, Exception? exception);
    void LogError(Exception exception);
    void LogCritical(string content);
}