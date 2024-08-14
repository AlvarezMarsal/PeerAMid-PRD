namespace PeerAMid.Utility;

/// <summary>
///     Used to indicate that a method is called from external code and should
///     not be removed or renamed carelessly.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
public class CalledFromExternalCodeAttribute : Attribute
{
    public CalledFromExternalCodeAttribute()
    {
    }
}