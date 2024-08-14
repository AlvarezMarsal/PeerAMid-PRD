#nullable enable

namespace PeerAMid.Data;

public class DemographicChartModel
{
    public decimal Minimum { get; set; }
    public decimal Percentile25 { get; set; }
    public decimal Percentile50 { get; set; }
    public decimal Percentile75 { get; set; }
    public decimal Maximum { get; set; }
    public decimal StarValue { get; set; }
    public decimal StarLocation { get; set; }
    public bool IsCAGRDiv0 { get; set; }
    public List<string> Outliers { get; } = new();

    public override string ToString()
    {
        return
            "Minimum: " + Minimum + ", " +
            "Percentile25: " + Percentile25 + ", " +
            "Percentile50: " + Percentile50 + ", " +
            "Percentile75: " + Percentile75 + ", " +
            "Maximum: " + Maximum + ", " +
            "StarValue: " + StarValue + ", " +
            "StarLocation:" + StarLocation + ", " +
            "Outliers: " + string.Join(", ", Outliers);
    }
}

public class DemographicModel
{
    public DemographicChartModel? RevenueData { get; set; }
    public DemographicChartModel? CAGRData { get; set; }
    public DemographicChartModel? GrossMarginData { get; set; }
    public DemographicChartModel? EBITDAData { get; set; }
    public DemographicChartModel? NumEmployeeData { get; set; }
    public DemographicChartModel? RevenuePerEmployeeData { get; set; }
    public DemographicChartModel? CashConversionCycle { get; set; }
    public string? Outliers { get; set; }

    public override string ToString()
    {
        return
            "Demographic Model\n" +
            " RevenueData:      " + RevenueData + "\n" +
            " CAGRData:         " + CAGRData + "\n" +
            " GrossMarginData:  " + GrossMarginData + "\n" +
            " EBITDAData:       " + EBITDAData + "\n" +
            " NumEmployeeData:  " + NumEmployeeData + "\n" +
            " RevPerEmployee:   " + RevenuePerEmployeeData + "\n" +
            " CCC:              " + RevenueData + "\n" +
            " Outliers: " + Outliers;
    }
}

public enum DemograpicChartSection
{
    Revenue = 1,
    CAGR = 2,
    GrossMargin = 3,
    EBITDA = 4,
    NumEmployee = 5,
    RevenuePerEmployee = 6,
    CashConversionCycle = 7
}
