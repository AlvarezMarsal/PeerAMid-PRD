namespace PeerAMid.Utility;

/// <summary>
///     This attribute represents a type's corresponding description for display purposes
/// </summary>
[AttributeUsage(AttributeTargets.All)]
public class DescriptionAttribute : Attribute
{
    public DescriptionAttribute(string value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string Value { get; }
}

/// <summary>
///     This attribute represents a type's report title for display purposes
/// </summary>
[AttributeUsage(AttributeTargets.All)]
public class ReportTitleAttribute : Attribute
{
    public ReportTitleAttribute(string value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string Value { get; }
}