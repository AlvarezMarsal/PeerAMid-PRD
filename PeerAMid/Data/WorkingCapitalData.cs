using Newtonsoft.Json;
using PeerAMid.Utility;
using System.Configuration;

#nullable enable

namespace PeerAMid.Data;

public class WorkingCapitalData
{
    public readonly SelectedTargetAndSubIndustriesModel SelectedData;

    public WorkingCapitalData(SelectedTargetAndSubIndustriesModel stasi)
    {
        SelectedData = stasi;
    }

    [JsonProperty(ItemIsReference = true)] public List<CompanyInfo> Companies { get; set; } = new();

    [JsonProperty(ItemIsReference = true)] public Dictionary<string, SummaryData> SummaryData { get; set; } = new();

    public CompanyInfo? FindCompany(int uid)
    {
        foreach (var i in Companies)
        {
            if (i.UID == uid)
                return i;
        }

        return null;
    }
}

public class SummaryData
{
    public List<SortedCompanyInfo> SortedCompanies { get; set; }

    public SummaryData(string name, bool isInverse, double p000, double p010,
        double p025, double p050, double p075, double p090, double p100,
        IEnumerable<CompanyInfo> companies)
    {
        Name = name;
        IsInverse = isInverse;

        P000 = p000;
        P010 = p010;
        P025 = p025;
        P050 = p050;
        P075 = p075;
        P090 = p090;
        P100 = p100;

        SortedCompanies = new List<SortedCompanyInfo>();
        foreach (var c in companies)
            SortedCompanies.Add(new SortedCompanyInfo(c, (double)c[Name]));
        var comparer = new SummaryCompanyInfoComparer<double>( /*Name,*/ isInverse);
        SortedCompanies.Sort(comparer);

        foreach (var sc in SortedCompanies)
        {
            if (sc.Company.IsTarget)
            {
                Target = sc;
                break;
            }
        }

        var m1 = SortedCompanies.Count / 2;
        var m2 = SortedCompanies.Count % 2 == 0 ? m1 - 1 : m1;
        SortedCompanies[m1].IsMedian = true;
        SortedCompanies[m2].IsMedian = true;

        var gap = (P075 - P025) * 1.5;
        LowerOutlierLimit = P025 - gap;
        UpperOutlierLimit = P075 + gap;

        //Log.Debug("LowerOutlierLimit: " + LowerOutlierLimit);
        //Log.Debug("UpperOutlierLimit: " + UpperOutlierLimit);

        var outliers = new List<string>();
        foreach (var sc in SortedCompanies)
        {
            var isOutlier = sc.Value < LowerOutlierLimit || sc.Value > UpperOutlierLimit;
            //Log.Debug(sc.Company.Name + ": " + sc.Value + " " + isOutlier);
            if (isOutlier)
            {
                sc.IsOutlier = true;
                outliers.Add(sc.Company.ShortName + " (" + Math.Round(sc.Value, 0) + " days)");
            }
        }

        var o = StringExtensionMethods.OxfordComma(outliers);
        if (outliers.Count == 1)
            o += " is an outlier.";
        else if (outliers.Count > 1) o += " are outliers.";
        if (outliers.Count > 0)
            o += "  Users are advised to omit outliers, as there may be issues with reported data.";
        OutliersDescription = o;
    }

    public string Name { get; }
    public bool IsInverse { get; }
    public double P000 { get; }
    public double P010 { get; }
    public double P025 { get; }
    public double P050 { get; }
    public double P075 { get; }
    public double P090 { get; }
    public double P100 { get; }
    public double TargetValue { get; /*private set;*/ }
    public SortedCompanyInfo? Target { get; }
    public double LowerOutlierLimit { get; }
    public double UpperOutlierLimit { get; }
    public string OutliersDescription { get; }

    public class SortedCompanyInfo
    {
        private static readonly string CommonColor;
        private static readonly string MedianColor;
        private static readonly string TargetColor;
        private static readonly string OutlierColor;

        [JsonProperty(IsReference = true)] public readonly CompanyInfo Company;

        public readonly double Value;

        static SortedCompanyInfo()
        {
            CommonColor = ConfigurationManager.AppSettings["sgaCommonColor"];
            MedianColor = ConfigurationManager.AppSettings["sgaMedianColor"];
            TargetColor = ConfigurationManager.AppSettings["sgaTargetCompanyColor"];
            OutlierColor = ConfigurationManager.AppSettings["sgaOutlierColor"];
        }

        internal SortedCompanyInfo(CompanyInfo company, double value)
        {
            Company = company;
            Value = value;
        }

        public bool IsMedian { get; internal set; }
        public bool IsOutlier { get; internal set; }
        public bool IsTarget => Company.IsTarget;

        public string DisplayName => Company.DisplayName;

        private string Color1 => IsMedian ? MedianColor : Color2;
        private string Color2 => IsOutlier ? OutlierColor : IsTarget ? TargetColor : IsMedian ? MedianColor : CommonColor;
        public string BorderColor => Color2;
        public string FillColor => Color1;
    }
}

internal class SummaryCompanyInfoComparer<F> : IComparer<SummaryData.SortedCompanyInfo>
    where F : IComparable<F>
{
    public readonly bool IsInverse;
    //public readonly string Name;

    public SummaryCompanyInfoComparer( /*string name,*/ bool isInverse)
    {
        //Name = name;
        IsInverse = isInverse;
    }

    public int Compare(SummaryData.SortedCompanyInfo x, SummaryData.SortedCompanyInfo y)
    {
        var c = x.Value.CompareTo(y.Value);
        if (c == 0)
            c = x.Company.Name.CompareTo(y.Company.Name);
        if (IsInverse)
            c = -c;
        return c;
    }
}
