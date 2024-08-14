using System.Data;

namespace PeerAMid.Utility;

public class DBDataHelper
{
    public static bool GetBoolean(IDataReader rdr, string columnName)
    {
        try
        {
            var index = rdr.GetOrdinal(columnName);
            return !rdr.IsDBNull(index) && Convert.ToBoolean(rdr[index]);
        }
        catch
        {
            return false;
        }
    }

    public static decimal GetDecimal(IDataReader rdr, string columnName)
    {
        try
        {
            var index = rdr.GetOrdinal(columnName);
            return !rdr.IsDBNull(index) ? Convert.ToDecimal(rdr[index]) : decimal.Zero;
        }
        catch
        {
            return 0;
        }
    }

    public static double GetDouble(IDataReader rdr, string columnName)
    {
        try
        {
            var index = rdr.GetOrdinal(columnName);
            if (rdr.IsDBNull(index)) return 0.0;
            return (double)rdr[index];
        }
        catch
        {
            return 0;
        }
    }

    public static double? GetNullableDouble(IDataReader rdr, string columnName)
    {
        try
        {
            var index = rdr.GetOrdinal(columnName);
            if (rdr.IsDBNull(index)) return null;
            return (double)rdr[index];
        }
        catch
        {
            return 0;
        }
    }

    public static int GetInt(IDataReader rdr, string columnName)
    {
        try
        {
            var index = rdr.GetOrdinal(columnName);
            if (rdr.IsDBNull(index)) return 0;
            var value = rdr[index];
            return (value is string s) ? int.Parse(s) : (int)value;
        }
        catch
        {
            return 0;
        }
    }

    public static int? GetNullableInt(IDataReader rdr, string columnName)
    {
        try
        {
            var index = rdr.GetOrdinal(columnName);
            if (rdr.IsDBNull(index)) return null;
            return (int)rdr[index];
        }
        catch
        {
            return null;
        }
    }

    public static string GetString(IDataReader rdr, string columnName)
    {
        try
        {
            var index = rdr.GetOrdinal(columnName);
            if (rdr.IsDBNull(index)) return string.Empty;
            return (string)rdr[index];
        }
        catch
        {
            return string.Empty;
        }
    }

    public static bool IsNull(IDataReader rdr, string columnName)
    {
        try
        {
            var index = rdr.GetOrdinal(columnName);
            return rdr.IsDBNull(index);
        }
        catch
        {
            return true;
        }
    }

    public static DateTime GetDateTime(IDataReader rdr, string columnName)
    {
        try
        {
            var index = rdr.GetOrdinal(columnName);
            if (rdr.IsDBNull(index)) return DateTime.MinValue;
            return rdr.GetDateTime(index);
        }
        catch
        {
            return DateTime.MinValue;
        }
    }

    public static void Get(IDataReader rdr, string columnName, out bool result)
    {
        result = GetBoolean(rdr, columnName);
    }

    public static void Get(IDataReader rdr, string columnName, out int result)
    {
        result = GetInt(rdr, columnName);
    }

    public static void Get(IDataReader rdr, string columnName, out string result)
    {
        result = GetString(rdr, columnName);
    }

    public static void Get(IDataReader rdr, string columnName, out double result)
    {
        result = GetDouble(rdr, columnName);
    }

    public static void Get(IDataReader rdr, string columnName, out DateTime result)
    {
        result = GetDateTime(rdr, columnName);
    }
}