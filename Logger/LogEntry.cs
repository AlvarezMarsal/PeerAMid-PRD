using System;
using System.Data.Common;
using System.Diagnostics;

#nullable enable

namespace Logger;

#pragma warning disable CA1812 // 'Log.LogEntry' is an internal class that is apparently never instantiated. If so, remove the code from the assembly. If this class is intended to contain only static members, make it 'static' (Module in Visual Basic). (https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1812)
internal class LogEntry
#pragma warning restore CA1812 // 'Log.LogEntry' is an internal class that is apparently never instantiated. If so, remove the code from the assembly. If this class is intended to contain only static members, make it 'static' (Module in Visual Basic). (https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1812)
{
    public static readonly LogEntryPool Pool = new();
    public static readonly int ProcessId;
    public static readonly string ProcessName;

    public string AppName = "";
    public string CallerFilePath = "";
    public int CallerLineNumber;
    public string CallerMemberName = "";
    public DbCommand? DbCommand;
    public string? ExceptionMessage;
    public string? ExceptionStackTrace;
    public string Message = "";
    public int ThreadId;
    public string ThreadName = "";
    public DateTime Time;
    public LogEntryType Type;

    static LogEntry()
    {
        var p = Process.GetCurrentProcess();
        ProcessId = p.Id;
        ProcessName = p.ProcessName;
    }

    public Exception? Exception
    {
        set
        {
            ExceptionMessage = value?.Message;
            ExceptionStackTrace = value?.StackTrace;
        }
    }

    /// <summary>
    ///     Log a text message
    /// </summary>
    /// <param name="type"></param>
    /// <param name="message"></param>
    /// <param name="callerMemberName"></param>
    /// <param name="callerFilePath"></param>
    /// <param name="callerLineNumber"></param>
    /// <returns></returns>
    public LogEntry Set(LogEntryType type, string? message,
        string? callerMemberName, string? callerFilePath, int callerLineNumber)
    {
        return Set(type, message, null, null, callerMemberName, callerFilePath, callerLineNumber);
    }


    /// <summary>
    ///     Log an exception and a text message
    /// </summary>
    /// <param name="type"></param>
    /// <param name="message"></param>
    /// <param name="ex"></param>
    /// <param name="callerMemberName"></param>
    /// <param name="callerFilePath"></param>
    /// <param name="callerLineNumber"></param>
    /// <returns></returns>
    public LogEntry Set(LogEntryType type, string? message, Exception ex,
        string? callerMemberName, string? callerFilePath, int callerLineNumber)
    {
        return Set(type, message, ex, null, callerMemberName, callerFilePath, callerLineNumber);
    }


    /// <summary>
    ///     Log an exception
    /// </summary>
    /// <param name="type"></param>
    /// <param name="ex"></param>
    /// <param name="callerMemberName"></param>
    /// <param name="callerFilePath"></param>
    /// <param name="callerLineNumber"></param>
    /// <returns></returns>
    public LogEntry Set(LogEntryType type, Exception ex,
        string? callerMemberName, string? callerFilePath, int callerLineNumber)
    {
        return Set(type, null, ex, null, callerMemberName, callerFilePath, callerLineNumber);
    }


    /// <summary>
    ///     Log a message and a database command
    /// </summary>
    /// <param name="type"></param>
    /// <param name="message"></param>
    /// <param name="dbCmd"></param>
    /// <param name="callerMemberName"></param>
    /// <param name="callerFilePath"></param>
    /// <param name="callerLineNumber"></param>
    /// <returns></returns>
    public LogEntry Set(LogEntryType type, string? message, DbCommand? dbCmd,
        string? callerMemberName, string? callerFilePath, int callerLineNumber)
    {
        return Set(type, message, null, dbCmd, callerMemberName, callerFilePath, callerLineNumber);
    }

    /// <summary>
    ///     Log a database command
    /// </summary>
    /// <param name="type"></param>
    /// <param name="dbCmd"></param>
    /// <param name="callerMemberName"></param>
    /// <param name="callerFilePath"></param>
    /// <param name="callerLineNumber"></param>
    /// <returns></returns>
    public LogEntry Set(LogEntryType type, DbCommand? dbCmd,
        string? callerMemberName, string? callerFilePath, int callerLineNumber)
    {
        return Set(type, null, null, dbCmd, callerMemberName, callerFilePath, callerLineNumber);
    }

    /// <summary>
    ///     Log a message, an exception, and a database command
    /// </summary>
    /// <param name="type"></param>
    /// <param name="message"></param>
    /// <param name="ex"></param>
    /// <param name="dbCmd"></param>
    /// <param name="callerMemberName"></param>
    /// <param name="callerFilePath"></param>
    /// <param name="callerLineNumber"></param>
    /// <returns></returns>
    public LogEntry Set(LogEntryType type, string? message, Exception? ex, DbCommand? dbCmd,
        string? callerMemberName, string? callerFilePath, int callerLineNumber)
    {
        Type = type;
        Message = message ?? "";
        Exception = ex;
        DbCommand = dbCmd;
        CallerMemberName = callerMemberName ?? "";
        CallerFilePath = callerFilePath ?? "";
        CallerLineNumber = callerLineNumber;
        return this;
    }

    public override string ToString()
    {
        return Message ?? "No message";
    }
}
