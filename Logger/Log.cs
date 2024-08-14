using Logger;
using System.Data.Common;
using System.IO;
using System.Runtime.CompilerServices;

namespace System;

#nullable enable

public static class Log
{
    private static readonly string DefaultApplicationName;
    public static string LogFolder { get; set; }


    private static LogRecorder? _recorder;
    private static string? _appName;

    static Log()
    {
        DefaultApplicationName = "PeerAMid";
        LogFolder = Path.GetTempPath(); // ConfigurationManager.AppSettings.GetForThisMachine("LogFolder") ?? Path.GetTempPath();
    }

    public static string ApplicationName
    {
        get => _appName ?? DefaultApplicationName;
        set
        {
            var an = value ?? DefaultApplicationName;
            if (an != _appName)
            {
                _appName = an;
                _recorder?.Dispose();
                _recorder = null;
            }
        }
    }

    private static LogRecorder Recorder
    {
        get
        {
            _recorder ??= new LogRecorder(LogFolder, ApplicationName);
            return _recorder;
        }
    }

    #region DEBUG output

    // Simple text
    public static void Debug(string message, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        Recorder.RecordLogEntry(
            LogEntryType.Debug,
            message,
            null,
            null,
            callerMemberName,
            callerFilePath,
            callerLineNumber);
    }

    // Text plus exception
    public static void Debug(string message, Exception exception, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        Recorder.RecordLogEntry(
            LogEntryType.Debug,
            message,
            exception,
            null,
            callerMemberName,
            callerFilePath,
            callerLineNumber);
    }

    // Exception
    public static void Debug(Exception exception, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        Recorder.RecordLogEntry(
            LogEntryType.Debug,
            null,
            exception,
            null,
            callerMemberName,
            callerFilePath,
            callerLineNumber);
    }

    // Dump a database command
    public static void Debug(DbCommand dbCmd, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        Recorder.RecordLogEntry(
            LogEntryType.Debug,
            null,
            null,
            dbCmd,
            callerMemberName,
            callerFilePath,
            callerLineNumber);
    }

    // Dump a file
    public static void DebugImportFile(string linePrefix, string filename, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        Recorder.RecordImportLogEntry(
            LogEntryType.Debug,
            linePrefix,
            filename,
            callerMemberName,
            callerFilePath,
            callerLineNumber);
    }
    #endregion

    #region INFO output

    // Simple text
    public static void Info(string message, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        Recorder.RecordLogEntry(
            LogEntryType.Info,
            message,
            null,
            null,
            callerMemberName,
            callerFilePath,
            callerLineNumber);
    }


    // Text plus exception
    public static void Info(string message, Exception exception, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        Recorder.RecordLogEntry(
            LogEntryType.Info,
            message,
            exception,
            null,
            callerMemberName,
            callerFilePath,
            callerLineNumber);
    }


    // Exception
    public static void Info(Exception exception, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        Recorder.RecordLogEntry(
            LogEntryType.Info,
            null,
            exception,
            null,
            callerMemberName,
            callerFilePath,
            callerLineNumber);
    }


    // Dump a database command
    public static void Info(DbCommand dbCmd, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        Recorder.RecordLogEntry(
            LogEntryType.Info,
            null,
            null,
            dbCmd,
            callerMemberName,
            callerFilePath,
            callerLineNumber);
    }

    #endregion

    #region WARN output

    // Simple text
    public static void Warn(string message, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        Recorder.RecordLogEntry(
            LogEntryType.Warning,
            message,
            null,
            null,
            callerMemberName,
            callerFilePath,
            callerLineNumber);
    }

    // Text plus exception
    public static void Warn(string message, Exception exception, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        Recorder.RecordLogEntry(
            LogEntryType.Warning,
            message,
            exception,
            null,
            callerMemberName,
            callerFilePath,
            callerLineNumber);
    }

    // Exception
    public static void Warn(Exception exception, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        Recorder.RecordLogEntry(
            LogEntryType.Warning,
            null,
            exception,
            null,
            callerMemberName,
            callerFilePath,
            callerLineNumber);
    }

    // Dump a database command
    public static void Warn(DbCommand dbCmd, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        Recorder.RecordLogEntry(
            LogEntryType.Warning,
            null,
            null,
            dbCmd,
            callerMemberName,
            callerFilePath,
            callerLineNumber);
    }

    #endregion

    #region ERROR output

    // Simple text
    public static void Error(string message, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        Recorder.RecordLogEntry(
            LogEntryType.Error,
            message,
            null,
            null,
            callerMemberName,
            callerFilePath,
            callerLineNumber);
    }

    // Text plus exception
    public static void Error(string message, Exception exception, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        Recorder.RecordLogEntry(
            LogEntryType.Error,
            message,
            exception,
            null,
            callerMemberName,
            callerFilePath,
            callerLineNumber);
    }

    // Exception
    public static void Error(Exception exception, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        Recorder.RecordLogEntry(
            LogEntryType.Error,
            null,
            exception,
            null,
            callerMemberName,
            callerFilePath,
            callerLineNumber);
    }

    // Dump a database command
    public static void Error(DbCommand dbCmd, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        Recorder.RecordLogEntry(
            LogEntryType.Error,
            null,
            null,
            dbCmd,
            callerMemberName,
            callerFilePath,
            callerLineNumber);
    }

    #endregion

    #region FATAL output

    // Simple text
    public static void Fatal(string message, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        Recorder.RecordLogEntry(
            LogEntryType.Emergency,
            message,
            null,
            null,
            callerMemberName,
            callerFilePath,
            callerLineNumber);
    }

    // Text plus exception
    public static void Fatal(string message, Exception exception, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        Recorder.RecordLogEntry(
            LogEntryType.Emergency,
            message,
            exception,
            null,
            callerMemberName,
            callerFilePath,
            callerLineNumber);
    }

    // Exception
    public static void Fatal(Exception exception, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        Recorder.RecordLogEntry(
            LogEntryType.Emergency,
            null,
            exception,
            null,
            callerMemberName,
            callerFilePath,
            callerLineNumber);
    }

    // Dump a database command
    public static void Fatal(DbCommand dbCmd, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        Recorder.RecordLogEntry(
            LogEntryType.Emergency,
            null,
            null,
            dbCmd,
            callerMemberName,
            callerFilePath,
            callerLineNumber);
    }

    #endregion
}
