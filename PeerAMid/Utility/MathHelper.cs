namespace PeerAMid.Utility;

public static class MathHelper
{
    public static T Bounded<T>(T value, T min, T max) where T : IComparable<T>
    {
        if (value.CompareTo(min) < 0)
            value = min;
        if (value.CompareTo(max) > 0)
            value = max;
        return value;
    }
}