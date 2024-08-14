using PeerAMid.Utility;
using System.Configuration;
using System.Text;
// ReSharper disable InconsistentNaming
#nullable enable
namespace PeerAMid.Data;

public class SGAModel
{
    public SelectedTargetAndSubIndustriesModel? SelectedData { get; set; }
    public ChartModel? ChartData { get; set; }
    public DifferenceTableModel<decimal>? DifferenceTableData { get; set; }
}

public class ChartModel
{
    public List<ChartProviderModel>? ProviderData { get; set; }
}

public static class Cutoffs
{
    public const int WorstDecileCutoff = 0; // anything worse than this is 'worst'
    public const int WorstQuartileCutoff = 1;
    public const int Median = 2;
    public const int BestQuartileCutoff = 3;
    public const int BestDecileCutoff = 4;
    public const int Count = 5;
}

public enum Ranking
{
    WorstDecile, // Cutoffs[WorstDecileCutoff]   >= x
    WorstQuartile, // Cutoffs[WorstQuartileCutoff] >= x > Cutoffs[WorstDecileCutoff]
    WorseThanMedian, // Cutoffs[Median] > x > Cutoffs[WorstQuartileCutoff]
    Median, // Cutoffs[Median] = x
    BetterThanMedian, // Cutoffs[BestQuartileCutoff] >= x > Cutoffs[Median]
    BestQuartile, // Cutoffs[BestDecileCutoff] >= x > Cutoffs[BestQuartileCutoff]
    BestDecile //                              x >= Cutoffs[BestDecileCutoff]
}

public class ChartProviderModel
{
    private static readonly string CommonColor;
    private static readonly string MedianColor;
    private static readonly string TargetColor;
    private static readonly string OutlierColor;
    private string? _displayName;

    static ChartProviderModel()
    {
        CommonColor = ConfigurationManager.AppSettings["sgaCommonColor"];
        MedianColor = ConfigurationManager.AppSettings["sgaMedianColor"];
        TargetColor = ConfigurationManager.AppSettings["sgaTargetCompanyColor"];
        OutlierColor = ConfigurationManager.AppSettings["sgaOutlierColor"];
    }

    public bool IsTarget { get; set; }
    public int UID { get; set; }
    public string? PeerCompanyName { get; set; }

    public string? PeerCompanyDisplayName
    {
        get => _displayName ?? PeerCompanyName;
        set => _displayName = value;
    }

    public decimal PeerCompanyValue { get; set; }

    private string Color1 => IsMedian ? MedianColor : Color2;
    private string Color2 => IsOutlier ? OutlierColor : IsTarget ? TargetColor : IsMedian ? MedianColor : CommonColor;
    public string BorderColor => Color2;
    public string FillColor => Color1;

    //public string ColorSecond { get; set; }

    public Ranking Ranking { get; set; }
    public bool IsMedian { get; set; }
    public bool IsOutlier { get; set; }

    public override string ToString()
    {
        return "UID=" + UID + " Name=" + PeerCompanyName;
    }
}

public class DifferenceTableModel<T> where T : notnull, IComparable<T>
{
    public readonly bool InvertedValues;

    public DifferenceTableModel(bool invertedValues)
    {
        InvertedValues = invertedValues;
    }

    public T CostMedian { get; set; } = default!;
    public T CostTarget { get; set; } = default!;
    public T CostTopQuartile { get; set; } = default!;
    public T CostTopDecile { get; set; } = default!;
    public T CostGapMedian { get; set; } = default!;
    public T CostGapTarget { get; set; } = default!;
    public T CostGapTopQuartile { get; set; } = default!;
    public T CostGapTopDecile { get; set; } = default!;
    public T OpportunityMedian { get; set; } = default!;
    public T OpportunityTopQuartile { get; set; } = default!;
    public T OpportunityTopDecile { get; set; } = default!;
    public T CostBottomQuartile { get; set; } = default!;
    public T CostBottomDecile { get; set; } = default!;
    public T CostMin { get; set; } = default!;
    public T CostMax { get; set; } = default!;
    public Ranking TargetRanking { get; set; }
    public string OutliersDescription { get; set; } = "";

    public Ranking DetermineRanking(T value /*, bool debug*/)
    {
        Ranking ranking;

        // Log.Debug($"Comparing {value} to {CostMedian}");

        if (value.CompareTo(CostMedian) < 0)
            ranking = Ranking.WorseThanMedian;
        else if (value.CompareTo(CostMedian) == 0)
            ranking = Ranking.Median;
        else if (value.CompareTo(CostTopQuartile) < 0)
            ranking = Ranking.Median;
        else if (value.CompareTo(CostTopDecile) < 0)
            ranking = Ranking.BestQuartile;
        else
            ranking = Ranking.BestDecile;

        // Log.Debug($"    ----> {ranking}");
        return ranking;
    }

    public override string ToString()
    {
        var b = new StringBuilder();
        AppendToString(b);
        return b.ToString();
    }

    public virtual void AppendToString(StringBuilder b, string indent = "")
    {
        b.Append("InvertedValues:     ").Append(InvertedValues).AppendLine();
        b.Append("CostMin:            ").Append(CostMin).AppendLine();
        b.Append("CostBottomDecile:   ").Append(CostBottomDecile).AppendLine();
        b.Append("CostBottomQuartile: ").Append(CostBottomQuartile).AppendLine();
        b.Append("CostMedian:         ").Append(CostMedian).AppendLine();
        b.Append("CostTopQuartile:    ").Append(CostTopQuartile).AppendLine();
        b.Append("CostTopDecile:      ").Append(CostTopDecile).AppendLine();
        b.Append("CostMax:            ").Append(CostMax).AppendLine();
        b.Append("CostTarget:    ").Append(CostTarget).AppendLine();
        b.Append("TargetRanking: ").Append(TargetRanking).AppendLine();
        b.Append("CostGapTarget: ").Append(CostGapTarget).AppendLine();
        b.Append("CostGapMedian:      ").Append(CostGapMedian).AppendLine();
        b.Append("CostGapTopQuartile: ").Append(CostGapTopQuartile).AppendLine();
        b.Append("CostGapTopDecile:   ").Append(CostGapTopDecile).AppendLine();
        b.Append("OpportunityMedian:      ").Append(OpportunityMedian).AppendLine();
        b.Append("OpportunityTopQuartile: ").Append(OpportunityTopQuartile).AppendLine();
        b.Append("OpportunityTopDecile:   ").Append(OpportunityTopDecile);
    }
}

public abstract class Breakdown<T> where T : notnull, IComparable<T>
{
    public readonly T[] CutoffValues; // always sorted lowest (index 0) to highest (index 6)
    public readonly bool InvertedValues;

    protected readonly double[] Percentiles =
    {
        0.10, 0.25, 0.50, 0.75, 0.90
    };

    public Breakdown(bool inverseValues)
    {
        CutoffValues = new T[Cutoffs.Count];
        InvertedValues = inverseValues;
    }

    public T TargetValue { get; set; } = default!;
    public double TargetPercentile { get; protected set; }

    //public T Worst
    //{
    //    get => Cutoffs[AdjustIndex(0)];
    //    set => Cutoffs[AdjustIndex(0)] = value;
    //}

    public T WorstDecileCutoff
    {
        get => CutoffValues[Cutoffs.WorstDecileCutoff];
        set => CutoffValues[Cutoffs.WorstDecileCutoff] = value;
    }

    public T WorstQuartileCutoff
    {
        get => CutoffValues[Cutoffs.WorstQuartileCutoff];
        set => CutoffValues[Cutoffs.WorstQuartileCutoff] = value;
    }

    public T Median
    {
        get => CutoffValues[Cutoffs.Median];
        set => CutoffValues[Cutoffs.Median] = value;
    }

    public T BestQuartileCutoff
    {
        get => CutoffValues[Cutoffs.BestQuartileCutoff];
        set => CutoffValues[Cutoffs.BestQuartileCutoff] = value;
    }

    public T BestDecileCutoff
    {
        get => CutoffValues[Cutoffs.BestDecileCutoff];
        set => CutoffValues[Cutoffs.BestDecileCutoff] = value;
    }

    public void SetCutoffs<S>(IEnumerable<S> items, Func<S, T> getter, S targetItem)
    {
        var values = new List<T>();
        foreach (var s in items)
        {
            var v = getter(s);
            values.Add(v);
        }

        values.Sort();

        CutoffValues[Cutoffs.WorstDecileCutoff] = CalculatePercentile(values, Percentiles[1]);
        CutoffValues[Cutoffs.WorstQuartileCutoff] = CalculatePercentile(values, Percentiles[2]);
        CutoffValues[Cutoffs.Median] = CalculatePercentile(values, Percentiles[3]);
        CutoffValues[Cutoffs.BestQuartileCutoff] = CalculatePercentile(values, Percentiles[4]);
        CutoffValues[Cutoffs.BestDecileCutoff] = CalculatePercentile(values, Percentiles[5]);

        TargetValue = getter(targetItem);

        for (var i = 0; i < CutoffValues.Length; i++)
        {
            if (CutoffValues[i].CompareTo(TargetValue) >= 0)
            {
                TargetPercentile = Interpolate(i - 1, i, TargetValue);
                break;
            }
        }
    }

    protected abstract T CalculatePercentile(IList<T> values, double fraction);

    public abstract double Interpolate(int indexLo, int indexHigh, T value);

    private int AdjustIndex(int index)
    {
        return InvertedValues ? CutoffValues.Length - 1 - index : index;
    }

    public override string ToString()
    {
        var b = new StringBuilder();
        AppendToString(b);
        return b.ToString();
    }

    public void AppendToString(StringBuilder b, string indent = "")
    {
        b.Append(indent).Append("InvertedValues  = ").Append(InvertedValues);
        for (var i = 0; i < CutoffValues.Length; i++)
            b.AppendLine().Append("CutoffValues[" + i + "] = ").Append(CutoffValues[i]);
    }
}

public class DecimalBreakdown : Breakdown<decimal>
{
    public DecimalBreakdown(bool inverseValues) : base(inverseValues)
    {
    }

    protected override decimal CalculatePercentile(IList<decimal> values, double fraction)
    {
        var index = fraction * values.Count;
        var f = Math.Floor(index);
        var i = (int)f;
        if (f.IsReasonablyEqual(index)) return (values[i - 1] + values[i]) / 2;
        return values[i];
    }

    public override double Interpolate(int indexLo, int indexHigh, decimal value)
    {
        var v0 = value - CutoffValues[indexLo];
        var v1 = CutoffValues[indexHigh] - CutoffValues[indexLo];
        var f = (double)(v0 / v1);
        var p0 = Percentiles[indexLo];
        var p1 = Percentiles[indexHigh] - Percentiles[indexLo];

        return p0 + f * p1;
    }
}

public class ChartParamModel : DecimalBreakdown
{
    public ChartParamModel(bool inverseValues) : base(inverseValues)
    {
    }

    public decimal P10Value
    {
        get => BestDecileCutoff;
        set => BestDecileCutoff = value;
    }

    public decimal P25Value
    {
        get => BestQuartileCutoff;
        set => BestQuartileCutoff = value;
    }

    public decimal P50Value
    {
        get => Median;
        set => Median = value;
    }

    public decimal P75Value
    {
        get => WorstQuartileCutoff;
        set => WorstQuartileCutoff = value;
    }

    public decimal P90Value
    {
        get => WorstDecileCutoff;
        set => WorstDecileCutoff = value;
    }

    public decimal TargetRevenue { get; set; }
}

public class ChartPPTModel
{
    public string? PeerCompanyName { get; set; }
    public decimal PeerCompanyValue { get; set; }
}
