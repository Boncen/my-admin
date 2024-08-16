using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyAdmin.Core.Entity;
using MyAdmin.Core.Extensions;
using MyAdmin.Core.Model.BuildIn;
using MyAdmin.Core.Repository;
using MyAdmin.Core.Utilities;

namespace MyAdmin.Core.Logger;

public class Logger : ILogger
{
    protected const string LogBasePath = "logs";
    protected string DefaultTemplate = @"{0}: 
stackTrace: {1}
    ";
    protected readonly LoggerOption _loggerOption;
    private readonly IRepository<Log> _repository;
    public Logger(IOptions<LoggerOption> loggerOption, IServiceScopeFactory serviceScopeFactory) //, IRepository<Log> repository)
    {
        _loggerOption = loggerOption.Value;
        // _repository = repository;
        if (_loggerOption.SaveToDatabase == true)
        {
            var scope = serviceScopeFactory.CreateScope();
            _repository = scope.ServiceProvider.GetRequiredService<IRepository<Log>>();
        }
    }
    protected string GetLocation()
    {
        var stackTrace = new StackTrace(true);
        var frame = stackTrace.GetFrame(3); // 获取调用 Log 方法的那一帧
        if (frame == null)
        {
            return string.Empty;
        }
        var method = frame.GetMethod();
        if (method == null)
        {
            return string.Empty;
        }
        var line = frame.GetFileLineNumber();
        string className = method!.ReflectedType!.Name;
        string methodName = method.Name;
        return className + "|" + methodName + "|" + line;
    }

    protected string FormatExtraInfo(LogLevel level, string stackTrace)
    {
        return string.Format(DefaultTemplate, level.ToString(), stackTrace);
    }

    public void LogCritical(string content)
    {
        Log(LogLevel.Critical, content, exception: null);
    }


    public void LogDebug(string content)
    {
        Log(LogLevel.Debug, content, exception: null);
    }


    public void LogError(string content, Exception? exception = null)
    {
        Log(LogLevel.Error, content, exception: exception);
    }

    public void LogError(Exception exception)
    {
        Log(LogLevel.Error, string.Empty, exception: exception);
    }

    public void LogInformation(string content)
    {
        Log(LogLevel.Information, content, exception: null);
    }

    public void LogTrace(string content)
    {
        Log(LogLevel.Trace, content, exception: null);
    }

    public void LogWarning(string content)
    {
        Log(LogLevel.Warning, content, exception: null);
    }


    protected void WriteToConsole(string content, LogLevel logLevel)
    {
        switch (logLevel)
        {
            case LogLevel.Trace:
            case LogLevel.Debug:
                Console.ResetColor();
                break;
            case LogLevel.Information:
                Console.ForegroundColor = ConsoleColor.Green;
                break;
            case LogLevel.Critical:
            case LogLevel.Error:
                Console.ForegroundColor = ConsoleColor.DarkRed;
                break;
            case LogLevel.Warning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            default:
                break;
        }
        Console.Write(content);
        Console.ResetColor();
    }

    public void Log(Log log)
    {
        if (log == null)
        {
            return;
        }
        if (_loggerOption.SaveToFile == true)
        {
            SaveLogToFileAsync(log.ToJsonString(), LogLevel.Trace);
        }

        if (_loggerOption.SaveToDatabase == true)
        {
            _repository.InsertAsync(log, true);
        }
    }

    public virtual void Log(LogLevel level, string content, Exception? exception = null)
    {
        string location = GetLocation();
        string extraInfo = FormatExtraInfo(level, location);
        WriteToConsole(extraInfo + content, level);
        
        if (_loggerOption.SaveToFile == true)
        {
            SaveLogToFileAsync(extraInfo + content + exception?.FullMessage(), level);
        } 
        if (_loggerOption.SaveToDatabase == true)
        {
            SaveLogToDatabaseAsync(content, level, exception);
        } 
    }

    protected void SaveLogToFileAsync(string log, LogLevel level)
    {
        if (!Check.HasValue(log))
        {
            return;
        }
        if (!Directory.Exists(LogBasePath))
        {
            Directory.CreateDirectory(LogBasePath);
        }
        var today = DateTime.Now;
        var filePath = LogBasePath + "/" + today.Year.ToString() + today.Month + today.Day + (_loggerOption.SplitFileViaLevel == true ? ("-" + level.ToString()) : string.Empty);
        if (_loggerOption.DatebasedDirectoryStructure == true)
        {
            string datebasedDir = LogBasePath + "/" + today.Year.ToString() + "/" + today.Month.ToString() + "/" + today.Day.ToString();
            if (!Directory.Exists(datebasedDir))
            {
                Directory.CreateDirectory(datebasedDir);
            }
            filePath = datebasedDir + "/" + (_loggerOption.SplitFileViaLevel == true ? level.ToString() : "log-all");
        }
        File.AppendAllText(filePath, log);
    }

    protected void SaveLogToDatabaseAsync(string log, LogLevel level, Exception? exception)
    {
        if (!Check.HasValue(log))
        {
            return;
        }

        _repository.InsertAsync(new Log()
        {
            Id = Guid.NewGuid(),
            Content = log,
            Level = level,
            LogTime = DateTime.Now,
            Exceptions = exception?.FullMessage()
        }, true);
    }
}

 