using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyAdmin.Core.Extensions;

namespace MyAdmin.Core.Logger;

public class Logger : ILogger
{
    private const string LogBasePath = "logs";
    private string DefaultTemplate = @"
{0} {1} : 
stackTrace: {2}
    {3}
    ";
    private readonly LoggerOption _loggerOption;
    public Logger(IOptions<LoggerOption> loggerOption)
    {
        _loggerOption = loggerOption.Value;
    }
    private string GetLocation()
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

    private string FormatContent(string content, LogLevel level, string stackTrace)
    {
        return string.Format(DefaultTemplate, level.ToString(), DateTime.Now.ToCommonString(), stackTrace, content);
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
        Log(LogLevel.Information, string.Empty, exception: null);
    }

    public void LogTrace(string content)
    {
        Log(LogLevel.Trace, string.Empty, exception: null);
    }

    public void LogWarning(string content)
    {
        Log(LogLevel.Warning, string.Empty, exception: null);
    }


    private void WriteToConsole(string content, LogLevel logLevel)
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

    public void Log(LogLevel level, string content, Exception? exception = null)
    {
        string location = GetLocation();
        string log = FormatContent(content + Environment.NewLine + exception == null ? string.Empty : exception!.FullMessage(), level, location);
        WriteToConsole(log, level);
        switch (_loggerOption.LogStorageType)
        {
            case StorageType.File:
                SaveLogToFileAsync(log, level);
                break;
            case StorageType.Database:

                break;
            case StorageType.MongoDB:

                break;
            default:
                break;
        }

    }

    private void SaveLogToFileAsync(string log, LogLevel level)
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
}