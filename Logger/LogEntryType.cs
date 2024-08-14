namespace Logger;

// Based on SYSLOG
internal enum LogEntryType
{
    Emergency = 0, //	System is unusable
    Alert = 1, //	Action must be taken immediately
    Critical = 2, //	Critical conditions
    Error = 3, //	Error conditions
    Warning = 4, //	Warning conditions
    Notice = 5, //	Normal but significant condition
    Info = 6, //	Informational messages
    Debug = 7 //	Debug-level messages
}

internal static class EntryTypes
{
    public const int Count = 8;

    public static readonly string[] Symbols =
    [
        "X", "A", "C", "E", "W", "N", "I", "D"
    ];
}