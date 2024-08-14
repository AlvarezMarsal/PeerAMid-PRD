using System;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Logger;

internal class LogEntryFormatter
{
    private static readonly char[] NewLineCharacters = Environment.NewLine.ToCharArray();
    private static readonly string Spaces = new(' ', 80);
    public static bool BreakLine = false;
    private readonly StringBuilder _builder = new();
    private readonly StringBuilder _msgbuilder = new();

    public string Format(in LogEntry entry)
    {
        _builder.Clear();
        _builder.Append(FormatDateTime(entry.Time)).Append(' ');
        _builder.Append(EntryTypes.Symbols[(int)entry.Type]).Append(' ');
        var indent = _builder.Length;
        _builder.Append('[').Append(entry.AppName).Append(':').Append(LogEntry.ProcessId).Append(' ');
        if (!string.IsNullOrEmpty(entry.ThreadName))
            _builder.Append(entry.ThreadName).Append(':');
        _builder.Append(entry.ThreadId).Append(']');

        if (BreakLine)
        {
            indent++;
            _builder.AppendLine().Append(Spaces, 0, indent);
        }
        else
        {
            _builder.Append(' ');
            indent = _builder.Length;
        }

        _msgbuilder.Clear();
        if (!string.IsNullOrWhiteSpace(entry.Message))
            _msgbuilder.Append(entry.Message);
        if (entry.ExceptionMessage != null)
        {
            if (_msgbuilder.Length > 0)
                _msgbuilder.AppendLine();
            _msgbuilder.Append(entry.ExceptionMessage);
        }

        if (entry.ExceptionStackTrace != null)
        {
            _msgbuilder.AppendLine();
            _msgbuilder.AppendLine().Append(entry.ExceptionStackTrace);
        }
        else if (entry.DbCommand != null)
        {
            if (_msgbuilder.Length > 0)
                _msgbuilder.AppendLine();
            if (entry.DbCommand.CommandType == CommandType.StoredProcedure)
                _msgbuilder.Append("Stored Procedure ");
            _msgbuilder.Append(entry.DbCommand.CommandText ?? "Stored Procedure");
            foreach (DbParameter p in entry.DbCommand.Parameters)
            {
                var line = "    " + p.ParameterName + " = ";
                if (p.Value == null)
                {
                    line += "NULL";
                }
                else
                {
                    line += p.DbType switch
                    {
                        DbType.Int16 => p.Value.ToString(),
                        DbType.Int32 => p.Value.ToString(),
                        DbType.Int64 => p.Value.ToString(),
                        DbType.Byte => p.Value.ToString(),
                        DbType.SByte => p.Value.ToString(),
                        DbType.UInt16 => p.Value.ToString(),
                        DbType.UInt32 => p.Value.ToString(),
                        DbType.UInt64 => p.Value.ToString(),
                        DbType.VarNumeric => p.Value.ToString(),
                        DbType.Currency => p.Value.ToString(),
                        DbType.Decimal => p.Value.ToString(),
                        DbType.Double => p.Value.ToString(),
                        DbType.Guid => p.Value.ToString(),
                        DbType.Single => p.Value.ToString(),
                        DbType.AnsiString => "'" + p.Value + "'",
                        DbType.Binary => "'" + p.Value + "'",
                        DbType.Boolean => "'" + p.Value + "'",
                        DbType.Date => "'" + p.Value + "'",
                        DbType.DateTime => "'" + p.Value + "'",
                        DbType.Object => "'" + p.Value + "'",
                        DbType.String => "'" + p.Value + "'",
                        DbType.Time => "'" + p.Value + "'",
                        DbType.AnsiStringFixedLength => "'" + p.Value + "'",
                        DbType.StringFixedLength => "'" + p.Value + "'",
                        DbType.Xml => "'" + p.Value + "'",
                        DbType.DateTime2 => "'" + p.Value + "'",
                        DbType.DateTimeOffset => "'" + p.Value + "'",
                        _ => "'" + p.Value + "'"
                    };
                }

                _msgbuilder.AppendLine().Append(line);
            }
        }

        var lines = _msgbuilder.ToString().Split(NewLineCharacters, StringSplitOptions.RemoveEmptyEntries);
        var count = 0;
        foreach (var line in lines)
        {
            if (count > 0)
                _builder.AppendLine().Append(Spaces, 0, indent);

            _builder.Append(line);
            ++count;
        }

        if (count > 1)
            _builder.AppendLine().Append(Spaces, 0, indent).Append("at ");
        else
            _builder.Append(" at ");
        _builder.Append(entry.CallerMemberName)
            .Append("() in ")
            .Append(entry.CallerFilePath)
            .Append(": line ")
            .Append(entry.CallerLineNumber);

        return _builder.ToString();
    }

    private static string FormatDateTime(DateTime d)
    {
        return d.ToString("MMdd HHmm ss.fff");
    }
}
