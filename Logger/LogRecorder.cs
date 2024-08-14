using System;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;


#nullable enable

namespace Logger;

internal class LogRecorder : IDisposable
{
    private const int AutoCloseSeconds = 15 * 60;
    protected static readonly byte[] NewLineBytes;
    private readonly int _creatingThreadId;
    public readonly bool AllowsFileSharing;
    public readonly string AppName;
    public readonly LogEntryFormatter Formatter;
    public readonly string LogFileName;
    protected readonly DateTime StartTime;
    private byte[] _buffer = new byte[1024];
    private ConcurrentQueue<LogEntry>? _pendingEntries;
    private ManualResetEventSlim? _readySignal;
    private ManualResetEventSlim? _stoppedSignal;
    private ManualResetEventSlim? _stopSignal;
    protected FileStream? LogFile;

    static LogRecorder()
    {
        NewLineBytes = Encoding.UTF8.GetBytes(Environment.NewLine);
    }

    public LogRecorder(string logFolder, string appName)
    {
        AllowsFileSharing = true;
        StartTime = DateTime.Now;
        AppName = appName ?? "App";
        Formatter = new LogEntryFormatter();

        if (string.IsNullOrEmpty(logFolder))
            logFolder = Path.GetTempPath();
        if (!Directory.Exists(logFolder))
            Directory.CreateDirectory(logFolder);

        LogFileName = DetermineFileName(logFolder, AppName, StartTime);
        UsingQueue = false;
        _creatingThreadId = Environment.CurrentManagedThreadId;

        OpenLogFile();
    }

    public bool UsingQueue { get; private set; }

    public void Dispose()
    {
        if (UsingQueue)
            _stopSignal?.Set();

        LogFile?.Flush();
        LogFile?.Dispose();
        LogFile = null;

        _stoppedSignal?.Dispose();
        _readySignal?.Dispose();
        _stopSignal?.Dispose();

        GC.SuppressFinalize(this);
    }

    private string DetermineFileName(string folder, string appName, DateTime startTime)
    {
        var lfn = Path.Combine(folder, appName + ".log");
        if (!File.Exists(lfn))
            return lfn;

        var end = File.GetLastWriteTime(lfn);
        string backupFilename;

        if (AllowsFileSharing)
        {
            if (end.DayOfYear == startTime.DayOfYear && end.Year == startTime.Year)
                return lfn;

            var dates = end.ToString("yyyy MMdd");
            backupFilename = Path.Combine(folder, Path.GetFileNameWithoutExtension(lfn) + " " + dates + ".log");
        }
        else
        {
            // Desired file already exists.  Try to back it up, and then reuse it.
            var start = File.GetCreationTime(lfn);

            var dates = start.ToString(" yyyy MMdd HHmmss - ");
            if (start.Year != end.Year)
                dates += end.ToString("yyyy MMdd HHmmss");
            else if (start.DayOfYear != end.DayOfYear)
                dates += end.ToString("MMdd HHmmss");
            else
                dates += end.ToString("HHmmss");

            backupFilename = Path.Combine(folder, Path.GetFileNameWithoutExtension(lfn) + " " + dates + ".log");
        }

        if (File.Exists(backupFilename))
        {
            AppendFileToFile(lfn, backupFilename);
        }
        else
        {
            try
            {
                File.Move(lfn, backupFilename);
            }
            catch
            {
            }
        }

        return lfn;
    }

    private void OpenLogFile(bool isReOpen = false)
    {
        LogFile = File.Open(LogFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        LogFile.Seek(0, SeekOrigin.End);

        if (LogFile.Position > 0)
        {
            LogFile.Seek(-1, SeekOrigin.Current);
            var lastByte = LogFile.ReadByte();
            if (lastByte != (byte)'\n')
                LogFile.Write(NewLineBytes, 0, NewLineBytes.Length);
        }

        if (!isReOpen)
        {
            var pageSeparator = Environment.NewLine +
                                new string('/', 180) +
                                Environment.NewLine +
                                Environment.NewLine;
            var pageSeparatorBytes = Encoding.UTF8.GetBytes(pageSeparator);
            LogFile.Write(pageSeparatorBytes, 0, pageSeparatorBytes.Length);
        }
    }

    private void ReOpenLogFile()
    {
        OpenLogFile(true);
    }

    private void StartUsingQueue()
    {
        UsingQueue = true;
        _pendingEntries = new ConcurrentQueue<LogEntry>();

        _stopSignal = new ManualResetEventSlim(false);
        _stoppedSignal = new ManualResetEventSlim(false);
        _readySignal = new ManualResetEventSlim(false);

        var thread = new Thread(LogThreadFunction)
        {
            Name = "Log"
        };
        thread.Start();
    }

    #region LogThreadFunction

    private void LogThreadFunction()
    {
        var fileCloseTime = DateTime.Now.AddSeconds(AutoCloseSeconds);

        while (!(_stopSignal?.IsSet ?? false))
        {
            _readySignal?.Wait(5000);
            if ((_pendingEntries?.Count ?? 0) > 0)
            {
                if (LogFile == null)
                    ReOpenLogFile();

                while (_pendingEntries?.TryDequeue(out var entry) ?? false)
                {
                    try
                    {
                        var s = Formatter.Format(entry);
                        RecordLogEntry(s);
                    }
                    catch (ThreadAbortException)
                    {
                        try
                        {
                            _stoppedSignal?.Set();
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }

                _readySignal?.Reset(); // probably unnecesary
                fileCloseTime = DateTime.Now.AddSeconds(AutoCloseSeconds);
            }
            else if (DateTime.Now >= fileCloseTime && LogFile != null)
            {
                LogFile.Flush();
                LogFile.Close();
                LogFile = null;
            }
        }
    }

    #endregion LogThreadFunction

    internal void RecordLogEntry(LogEntry entry)
    {
        if (UsingQueue)
        {
            _pendingEntries?.Enqueue(entry);
            _readySignal?.Set();
        }
        else if (Thread.CurrentThread.ManagedThreadId == _creatingThreadId)
        {
            RecordLogEntry(Formatter.Format(in entry));
        }
        else
        {
            StartUsingQueue();
            _pendingEntries?.Enqueue(entry);
            _readySignal?.Set();
        }
    }


    internal void RecordLogEntry(LogEntryType type, string? message, Exception? ex, DbCommand? dbCmd,
        string? callerMemberName, string? callerFilePath, int callerLineNumber)
    {
        RecordLogEntry(
            LogEntry.Pool.Get().Set(
                type,
                message,
                ex,
                dbCmd,
                callerMemberName,
                callerFilePath,
                callerLineNumber));
    }

    internal void RecordImportLogEntry(LogEntryType type, string linePrefix, string filename,
        string? callerMemberName, string? callerFilePath, int callerLineNumber)
    {
        Exception? ex = null;
        var text = "";
        try
        {
            if (string.IsNullOrEmpty(linePrefix))
            {
                text = "Imported file: " + filename + "\n" +
                        File.ReadAllText(filename) +
                       "End of imported file: " + filename;
            }
            else
            {
                var lines = File.ReadAllLines(filename);
                for (var i = 0; i < lines.Length; ++i)
                    lines[i] = linePrefix + lines[i];
                text = "Imported file: " + filename + "\n" +
                       string.Join("\n", lines) + "\n" +
                       "End of imported file: " + filename;
            }
        }
        catch (Exception e)
        {
            ex = e;
            text = "Failed to import file into log '" + filename + "'";
        }
        finally
        {
            RecordLogEntry(type, text, ex, null, callerMemberName, callerFilePath, callerLineNumber);
        }
    }

    private void RecordLogEntry(string entry)
    {
        if (LogFile != null)
        {
            try
            {
                var length = ToBytes(entry, ref _buffer);

                if (AllowsFileSharing)
                    LogFile?.Lock(0, 1);

                LogFile?.Write(_buffer, 0, length);
                LogFile?.Flush();

                if (AllowsFileSharing)
                    LogFile?.Unlock(0, 1);
            }
            catch
            {
                if (AllowsFileSharing)
                    LogFile?.Unlock(0, 1);
                LogFile = null;
            }
        }
    }

    private int ToBytes(string entry, ref byte[] bytes)
    {
        var count = Encoding.UTF8.GetByteCount(entry) + NewLineBytes.Length;
        if (bytes.Length < count)
            bytes = new byte[Math.Max(4096, count) * 2];
        var byteCount = Encoding.UTF8.GetBytes(entry, 0, entry.Length, bytes, 0);
        Array.Copy(NewLineBytes, 0, bytes, byteCount, NewLineBytes.Length);
        return count;
    }

    private static bool AppendFileToFile(string sourceFilename, string targetFilename)
    {
        try
        {
            /*using (*/
            var b = File.Open(targetFilename, FileMode.Append, FileAccess.ReadWrite); /*)*/
            {
                using var bsr = new StreamWriter(b);
                b.Seek(0, SeekOrigin.End);
                using var f = File.OpenText(sourceFilename);
                while (true)
                {
                    var line = f.ReadLine();
                    if (line == null)
                        break;
                    bsr.WriteLine(line);
                }

                return true;
            }
        }
        catch
        {
            //Debug.WriteLine(LogEntry.FormatMessage("Could not back up  " + sourceFilename));
        }

        return false;
    }
}
