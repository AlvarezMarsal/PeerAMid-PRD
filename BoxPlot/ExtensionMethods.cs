using System;

namespace BoxPlot;

internal static class ExtensionMethods
{
    public static bool IsVeryCloseTo(this double d1, double d2, double tolerance = 0.0001)
    {
        return Math.Abs(d1 - d2) < tolerance;
    }

    public static long ClosestInteger(this double d)
        => ClosestInteger(d, out _);

    public static long ClosestInteger(this double d, out double error)
    {
        var low = Math.Floor(d);
        var high = Math.Ceiling(d);
        var elow = d - low;
        var dlow = Math.Abs(elow);
        var ehigh = high - d;
        var dhigh = Math.Abs(ehigh);
        if (dlow < dhigh)
        {
            error = elow;
            return (long)dlow;
        }
        error = ehigh;
        return (long)high;

    }
}
