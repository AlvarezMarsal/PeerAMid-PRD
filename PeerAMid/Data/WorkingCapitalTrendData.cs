#nullable enable

namespace PeerAMid.Data;

public class WorkingCapitalTrendData
{
    public readonly SortedList<int, AnnualData> AnnualData = new();

    public WorkingCapitalTrendData(int firstYear, int lastYear)
    {
        AnnualData = new SortedList<int, AnnualData>();
        for (var year = firstYear; year <= lastYear; year++)
            AnnualData.Add(year, new AnnualData(year));
    }
}

public class AnnualData
{
    public readonly int Year;

    public AnnualData(int year)
    {
        Year = year;
    }

    public double? CCC { get; set; }
    public double? DIO { get; set; }
    public double? DPO { get; set; }
    public double? DSO { get; set; }
    public bool HasData => CCC.HasValue || DIO.HasValue || DPO.HasValue;
}
