namespace PeerAMid.Utility;

public static class NumericExtensionMethods
{
    public static bool IsReasonablyEqual(this double a, double b, double c = 0.001)
    {
        return Math.Abs(a - b) < c;
    }
}