using System.Reflection;

namespace PeerAMid.Utility;

/// <summary>
///     Exension Methods
/// </summary>
public static class EnumExtensionMethods
{
    public static TAttribute GetAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
    {
        var type = value.GetType();
        var name = Enum.GetName(type, value);
        return type.GetField(name).GetCustomAttribute<TAttribute>();
    }

    /// <summary>
    ///     Get the value of the Description attribute corresponding to the specified Enum value
    /// </summary>
    /// <param name="value">Enum value</param>
    /// <returns></returns>
    public static string GetDescription(this Enum value)
    {
        var charCode = value.GetAttribute<DescriptionAttribute>().Value;
        return charCode;
    }

    /// <summary>
    ///     Get the value of the Report Title attribute corresponding to the specified Enum value
    /// </summary>
    /// <param name="value">Enum value</param>
    /// <returns></returns>
    public static string GetReportTitle(this Enum value)
    {
        var charCode = value.GetAttribute<ReportTitleAttribute>().Value;
        return charCode;
    }
}