namespace PeerAMid.Business;

#nullable enable

public class CurrencySettings : IEquatable<CurrencySettings>
{
    public static readonly CurrencySettings Database = new();
    public static readonly CurrencySettings Default = new();

    public CurrencySettings(string currency = "USD", string uom = ConstantUnitOfMeasurement.Millions, double exchangeRate = 1)
    {
        Currency = currency;
        UnitOfMeasure = uom;
        ExchangeRate = exchangeRate;
    }

    public string Currency { get; set; }
    public string UnitOfMeasure { get; set; }
    public double ExchangeRate { get; set; }

    private int Scale
    {
        get
        {
            if (UnitOfMeasure == ConstantUnitOfMeasurement.Millions)
                return 6;
            if (UnitOfMeasure == ConstantUnitOfMeasurement.Billions)
                return 9;
            if (UnitOfMeasure == ConstantUnitOfMeasurement.Thousands)
                return 3;
            if (UnitOfMeasure == ConstantUnitOfMeasurement.WholeNumbers)
                return 0;
            return 6;
        }
    }

    public bool Equals(CurrencySettings other)
    {
        if (ReferenceEquals(null, other)) return true;
        return Currency == other.Currency &&
               UnitOfMeasure == other.UnitOfMeasure &&
               CloseEnough(ExchangeRate, other.ExchangeRate);
    }

    public CurrencySettings Clone()
    {
        return new CurrencySettings(Currency, UnitOfMeasure, ExchangeRate);
    }

    private static bool CloseEnough(double xr1, double xr2)
    {
        return Math.Abs(xr1 - xr2) <= 10E-6;
    }

    public override string ToString()
    {
        return (Currency ?? "???") + ":" + (UnitOfMeasure ?? "???") + ":" + ExchangeRate;
    }

    public double ConvertFrom(double value, CurrencySettings other)
    {
        var conversionFactor = 1.0;
        if (!CloseEnough(ExchangeRate, other.ExchangeRate))
            conversionFactor = other.ExchangeRate / ExchangeRate;

        if (UnitOfMeasure != other.UnitOfMeasure)
        {
            var scale = other.Scale - Scale;

            if (scale != 0)
                conversionFactor *= Math.Pow(10.0, scale);
        }

        var v = conversionFactor * value;
        Debug.WriteLine($"Converted from {other.Currency} {value} to {Currency} {v}");
        return v;
    }

    public void ConvertFrom(ref double value, CurrencySettings other)
    {
        value = ConvertFrom(value, other);
    }

    public double ConvertTo(double value, CurrencySettings other)
    {
        return other.ConvertFrom(value, this);
    }

    public void ConvertTo(ref double value, CurrencySettings other)
    {
        value = ConvertTo(value, other);
    }

    public double? ConvertFrom(double? valueMaybe, CurrencySettings other)
    {
        return valueMaybe.HasValue ? ConvertFrom(valueMaybe.Value, other) : null;
    }

    public void ConvertFrom(ref double? value, CurrencySettings other)
    {
        value = ConvertFrom(value, other);
    }

    public double? ConvertTo(double? value, CurrencySettings other)
    {
        return other.ConvertFrom(value, this);
    }

    public void ConvertTo(ref double? value, CurrencySettings other)
    {
        value = ConvertTo(value, other);
    }

    public decimal ConvertFrom(decimal value, CurrencySettings other)
    {
        var d = (double)value;
        var e = ConvertFrom(d, other);
        return (decimal)e;
    }

    public void ConvertFrom(ref decimal value, CurrencySettings other)
    {
        value = ConvertFrom(value, other);
    }

    public decimal ConvertTo(decimal value, CurrencySettings other)
    {
        return other.ConvertFrom(value, this);
    }

    public void ConvertTo(ref decimal value, CurrencySettings other)
    {
        value = ConvertTo(value, other);
    }

    public decimal? ConvertFrom(decimal? valueMaybe, CurrencySettings other)
    {
        return valueMaybe.HasValue ? ConvertFrom(valueMaybe.Value, other) : null;
    }

    public void ConvertFrom(ref decimal? value, CurrencySettings other)
    {
        value = ConvertFrom(value, other);
    }

    public decimal? ConvertTo(decimal? value, CurrencySettings other)
    {
        return other.ConvertFrom(value, this);
    }

    public void ConvertTo(ref decimal? value, CurrencySettings other)
    {
        value = ConvertTo(value, other);
    }
}
