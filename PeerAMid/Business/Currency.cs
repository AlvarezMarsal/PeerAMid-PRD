using System.Globalization;

namespace PeerAMid.Business;

#nullable enable

public class Currency
{
    private static readonly CultureInfo USA = new("en-US");
    public string Name { get; set; } = "???"; // 'USD'
    public string Description { get; set; } = ""; // 'United States Dollars'
    public string SmallValueLabel { get; set; } = ""; // ' 'USD'
    public string SmallValueFormat { get; set; } = "*"; // ' '$*'
    public int SmallValueDecimalPlaces { get; set; } = 0;
    public decimal LargeValueDivisor { get; set; } = 1000000;
    public string LargeValueLabel { get; set; } = ""; // 'USD MM'
    public string LargeValueFormat { get; set; } = "*"; // '$*MM'
    public int LargeValueDecimalPlaces { get; set; } = 2;
    public int AmericanFormatting { get; set; } // 1 for USD, 0 for others  (1,000,000.00 vs. 1.000.000,00)
    public int CanBeUsedForReports { get; set; } // 1 for USD, EUR, GBP, 0 for others

    // Needs to be Json - able
    public List<YearlyExchangeRate> ExchangeRateByYear { get; set; } = new();

    public string FormatSmallValue(decimal d)
    {
        return Format(d, SmallValueDecimalPlaces, SmallValueFormat);
    }

    public string FormatSmallValue(decimal d, int decimalPlaces)
    {
        return Format(d, decimalPlaces, SmallValueFormat);
    }

    public string FormatLargeValue(decimal d)
    {
        return Format(d, LargeValueDecimalPlaces, LargeValueFormat);
    }

    public string FormatLargeValue(decimal d, int decimalPlaces)
    {
        return Format(d, decimalPlaces, LargeValueFormat);
    }

    private string Format(decimal d, int decimalPlaces, string format)
    {
        var s = d.ToString("F" + decimalPlaces, USA);
        if (AmericanFormatting == 0)
            s = s.Replace(',', '!').Replace('.', ',').Replace('!', '.');
        s = format.Replace("*", s);
        return s;
    }

    public double GetExchangeRate(int year)
    {
        for (var i = 0; i < ExchangeRateByYear.Count; i++)
        {
            if (ExchangeRateByYear[i].Year == year)
                return ExchangeRateByYear[i].ExchangeRate;
        }

        return 1;
    }

    public override string ToString()
    {
        return Name;
    }
}

public struct YearlyExchangeRate
{
    public int Year { get; set; }
    public double ExchangeRate { get; set; }
}
