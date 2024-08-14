using System.Data;

namespace PeerAMid.Utility;

public static class DataReaderExtensions
{
    public static decimal? ToDecimal(this IDataReader reader, int column)
    {
        var type = reader.GetFieldType(column);
        var value = reader.GetValue(column);
        decimal d;
        // Log.Debug("column " + reader.GetName(column) + " : " + type.Name);
        if (value == DBNull.Value)
            return null;
        switch (type.Name)
        {
            case "Decimal":
                if (decimal.TryParse((string)value, out d))
                    return d;
                break;

            case "Double":
                return (decimal)(double)value;

            case "Single":
                return (decimal)(float)value;

            case "String":
                if (decimal.TryParse((string)value, out d))
                    return d;
                break;

            default:
                if (decimal.TryParse(value.ToString(), out d))
                    return d;
                break;
        }

        return null;
    }

    public static string ToString(this IDataReader reader, int column)
    {
        var type = reader.GetFieldType(column);
        var value = reader.GetValue(column);
        if (value == DBNull.Value)
            return "";
        switch (type.Name)
        {
            default:
                return value.ToString();
        }

        // return null;
    }

    public static int? ToInt(this IDataReader reader, int column)
    {
        var type = reader.GetFieldType(column);
        var value = reader.GetValue(column);
        if (value == DBNull.Value)
            return null;
        switch (type.Name)
        {
            case "Int32":
                return (int)value;

            case "String":
                if (int.TryParse((string)value, out var j))
                    return j;
                break;

            default:
                if (int.TryParse(value.ToString(), out var i))
                    return i;
                break;
        }

        return null;
    }

    public static bool? ToBoolean(this IDataReader reader, int column)
    {
        var type = reader.GetFieldType(column);
        var value = reader.GetValue(column);
        if (value == DBNull.Value)
            return null;
        switch (type.Name)
        {
            case "Boolean":
                return (bool)value;

            case "Int32":
                return (int)value != 0;

            default:
                {
                    var s = value.ToString();
                    if (s.Length == 0)
                        break;
                    if (s[0] == 't' || s[0] == 'T' || s[0] == 'y' || s[0] == 'Y')
                        return true;
                    if (s[0] == 'f' || s[0] == 'F' || s[0] == 'n' || s[0] == 'N')
                        return false;
                }
                break;
        }

        return null;
    }

    public static double? ToDouble(this IDataReader reader, int column)
    {
        var type = reader.GetFieldType(column);
        var value = reader.GetValue(column);
        decimal d;
        double dd;
        // Log.Debug("column " + reader.GetName(column) + " : " + type.Name);
        if (value == DBNull.Value)
            return null;
        switch (type.Name)
        {
            case "Decimal":
                if (decimal.TryParse((string)value, out d))
                    return (double)d;
                break;

            case "Double":
                return (double)value;

            case "Single":
                return (double)(float)value;

            case "String":
                if (double.TryParse((string)value, out dd))
                    return dd;
                break;

            default:
                if (double.TryParse(value.ToString(), out dd))
                    return dd;
                break;
        }

        return null;
    }
}
