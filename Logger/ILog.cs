using System;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace Logger;

#nullable enable

public interface ILog
{
    string ApplicationName { get; set; }

    #region DEBUG output

    // Simple text
    void Debug(string message, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0);

    // Text plus exception
    void Debug(string message, Exception exception, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0);

    // Exception
    void Debug(Exception exception, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0);

    // Dump a database command
    void Debug(DbCommand dbCmd, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0);

    // Import an external file into the log
    void DebugImportFile(string filename, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0); 

    // Import an external file into the log
    void DebugImportFile(string linePrefix, string filename, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0);

    #endregion

    #region INFO output

    // Simple text
    void Info(string message, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0);

    // Text plus exception
    void Info(string message, Exception exception, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0);

    // Exception
    void Info(Exception exception, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0);

    // Dump a database command
    void Info(DbCommand dbCmd, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0);

    #endregion

    #region WARN output

    // Simple text
    void Warn(string message, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0);

    // Text plus exception
    void Warn(string message, Exception exception, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0);

    // Exception
    void Warn(Exception exception, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0);

    // Dump a database command
    void Warn(DbCommand dbCmd, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0);

    #endregion

    #region ERROR output

    // Simple text
    void Error(string message, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0);

    // Text plus exception
    void Error(string message, Exception exception, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0);

    // Exception
    void Error(Exception exception, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0);

    // Dump a database command
    void Error(DbCommand dbCmd, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0);

    #endregion

    #region FATAL output

    // Simple text
    void Fatal(string message, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0);

    // Text plus exception
    void Fatal(string message, Exception exception, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0);

    // Exception
    void Fatal(Exception exception, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0);

    // Dump a database command
    void Fatal(DbCommand dbCmd, [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0);

    #endregion
}
