using Microsoft.Extensions.ObjectPool;
using System;
using System.Threading;

namespace Logger;

internal class LogEntryPool : DefaultObjectPool<LogEntry>
{
    public LogEntryPool() : base(new LogEntryPoolPolicy())
    {
    }

    public override LogEntry Get()
    {
        var entry = base.Get();
        entry.AppName = Log.ApplicationName;
        entry.ThreadId = Environment.CurrentManagedThreadId;
        entry.ThreadName = Thread.CurrentThread.Name ?? "";
        entry.Time = DateTime.Now;
        return entry;
    }
}

internal class LogEntryPoolPolicy : DefaultPooledObjectPolicy<LogEntry>
{
}