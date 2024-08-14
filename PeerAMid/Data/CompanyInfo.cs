using PeerAMid.Utility;
using System.Data;
using System.Text;

#nullable enable

namespace PeerAMid.Data;

public partial class CompanyInfo
{
    public static int NumberOfCompanies;

    private static double dsouq = double.NaN;

    private static double diouq = double.NaN;

    private static double dpouq = double.NaN;

    public readonly int CompanyIndex;
    private readonly WorkingCapitalData workingCapitalData;

    private string? _displayName;
    public string DataEntryCurrency = "USD";
    public double DataEntryExchangeRate = 1;
    public string DataEntryUnitOfMeasure = "Millions";

    public CompanyInfo(WorkingCapitalData wcd, int companyIndex, int uid, string name, bool isTarget, string displayName)
    {
        workingCapitalData = wcd;
        CompanyIndex = companyIndex;
        Uid_D = Uid_DM = uid;
        Coname = name.ToUpper();
        IsTarget = isTarget;
        DisplayName = displayName;
    }

    public string Name => Coname;
    public int UID => (int)Uid_D;

    public string DisplayName
    {
        get => _displayName ?? ShortNameMixedCase ?? CompanyNameMixedCase ?? Name ?? $"Missing display name for {UID}";
        set => _displayName = value;
    }

    public override string ToString()
    {
        var b = new StringBuilder();
        AppendToString(b);
        return b.ToString();
    }

    private void AppendToString(StringBuilder b, string indent = "")
    {
        b.Append(indent).Append("UID = ").Append(UID).AppendLine();
        b.Append(indent).Append("IsTarget = ").Append(IsTarget).AppendLine();
        b.Append(indent).Append("Name = ").Append(Name);
    }

    public static CompanyInfo? CreateFromDataReader(WorkingCapitalData wcd, int companyIndex, IDataReader reader, int uid)
    {
        CompanyInfo info;

        try
        {
            DBDataHelper.Get(reader, "Name", out string name);
            DBDataHelper.Get(reader, "IsTarget", out bool isTarget);
            DBDataHelper.Get(reader, "ShortName", out string shortName);
            DBDataHelper.Get(reader, "ShortNameMixedCase", out string displayName);
            DBDataHelper.Get(reader, "DataEntryCurrency", out string dataEntryCurrency);
            DBDataHelper.Get(reader, "DataEntryUnitOfMeasure", out string dataEntryUnitOfMeasure);
            DBDataHelper.Get(reader, "DataEntryExchangeRate", out double dataEntryExchangeRate);
            info = new CompanyInfo(wcd, companyIndex, uid, name, isTarget, displayName);
            info.ShortName = info.ShortName ?? shortName;
            info.DataEntryCurrency = dataEntryCurrency;
            info.DataEntryUnitOfMeasure = dataEntryUnitOfMeasure;
            info.DataEntryExchangeRate = dataEntryExchangeRate;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return null;
        }

        UpdateFromDataReader(info, reader);
        return info;
    }

    public static CompanyInfo UpdateFromDataReader(CompanyInfo info, IDataReader reader)
    {
        var i = 0;
        try
        {
            for (i = 0; i < reader.FieldCount; ++i)
            {
                if (reader.IsDBNull(i))
                    continue;
                var value = reader.GetValue(i);
                if (value == null)
                    continue;
                var fname = reader.GetName(i);
                // Log.Debug("Got '" + fname + "' as " + reader.GetDataTypeName(i) + " = " + value?.ToString() ?? "null");

                switch (fname)
                {
                    case "AP":
                        info.Ap = (double)value;
                        break;

                    case "AR":
                        info.Ar = (double)value;
                        break;

                    case "CAGR":
                        info.CAGR = (double)value;
                        break;

                    // case "CASALES": info.Casales_CK = (double)value; break;      // Now calculated
                    case "Cash_Equiv":
                        info.CashEquiv = (double)value;
                        break;

                    // case "CATA": info.Cata = (double)value; break;               // Now calculated
                    // case "CCRATIO": info.Ccratio = (double)value; break;         // Now calculated
                    // case "CL_EQ": info.ClEq = (double)value; break;              // Now calculated
                    case "COGS":
                        info.Cogs = (double)value;
                        break;

                    case "CompanyNameMixedCase":
                        info.CompanyNameMixedCase = (string)value;
                        break;

                    case "Country":
                        info.Country = (string)value;
                        break;

                    case "Curr_Assets":
                        info.CurrAssets = (double)value;
                        break;

                    case "Curr_Liab":
                        info.CurrLiab = (double)value;
                        break;

                    case "DataYear":
                        info.Datayear = (int)value;
                        break;

                    // case "DBTEQ": info.Dbteq = (double)value; break;
                    // case "DBTTA": info.Dbtta = (double)value; break;             // Now calculated
                    case "DSO":
                        info.Dso = (double)value;
                        break;

                    case "EBIT":
                        info.Ebit = (double)value;
                        break;

                    case "EBITDA":
                        info.Ebitda1 = (double)value;
                        break;

                    // case "EBITTA": info.Ebitta = (double)value; break;           // Now calculated
                    case "EM1":
                        info.Em1 = (double)value;
                        break;

                    // case "EQTURNS": info.Eqturns_CG = (double)value; break;      // Now calculated
                    // case "FAEQLTL": info.Faeqltl_CY = (double)value; break;      // Now calculated
                    // case "FATURN": info.Faturn_CE = (double)value; break;        // Now calculated
                    // case "FNMULTI": info.Fnmulti_CX = (double)value; break;      // Now calculated
                    // case "GM1": info.Gm1 = (double)value; break;                 // Now calculated
                    case "GP":
                        info.Gp = (double)value;
                        break;

                    // case "INVCA": info.Invca = (double)value; break;             // Now calculated
                    case "Inventory":
                        info.Inventory = (double)value;
                        break;

                    // case "Invsales": info.Invsales = (double)value; break;       // Now calculated
                    // case "ITURNS": info.Iturns = (double)value; break;           // Now calculated
                    //case "MKTSALES": info.Mktsales = (double)value; break;
                    //case "MKTTA": info.Mktta = (double)value; break;
                    //case "MKTTEQ": info.Mktteq = (double)value; break;
                    case "NI":
                        info.Ni = (double)value;
                        break;

                    case "NoOfEmployees":
                        info.Ee = (int)value;
                        break;

                    // case "NP1": info.Np1_BM = (double)value; break;              // Now calculated
                    case "NPPE":
                        info.Nppe = (double)value;
                        break;

                    // case "QASALES": info.Qasales_CJ = (double)value; break;      // Now calculated
                    // case "QTA": info.Qta = (double)value; break;                 // Now calculated
                    case "RetainedE":
                        info.Retainede = (double)value;
                        break; // Now calculated

                    case "REV1":
                        info.Rev1 = (double)value;
                        break;

                    case "RevPerEE":
                        info.Revperee = (double)value;
                        break;

                    // case "ROA": info.Roa = (double)value; break;                 // Now calculated
                    // case "ROE": info.Roe = (double)value; break;                 // Now calculated
                    // case "RTURNS": info.Rturns = (double)value; break;           // Now calculated
                    case "SGM1":
                        info.Sgm1 = (double)value;
                        break;

                    case "ShortNameMixedCase":
                        info.ShortNameMixedCase = (string)value;
                        // Log.Debug("Read SNMC as " + info.ShortNameMixedCase);
                        break;

                    case "SIC_Code":
                        info.SicCode = (string)value;
                        break;

                    case "SIC2D":
                        info.Sic2D = (string)value;
                        break;

                    case "SIC2D_Description":
                        info.Sic2DDescription = (string)value;
                        break;

                    case "SubIndustry":
                        info.SubIndustry = (string)value;
                        break;

                    // case "TATURN": info.Taturn = (double)value; break;           // Now calculated
                    case "T_Liab":
                        info.TLiab = (double)value;
                        break;

                    case "Total_Assets":
                        info.TotalAssets = (double)value;
                        break;

                    case "Total_EQUITY":
                        info.TotalEquity = (double)value;
                        break;

                    case "WCAP":
                        info.Wcap = (double)value;
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(
                "Failed at '" +
                reader.GetName(i) +
                "' as " +
                reader.GetDataTypeName(i) +
                " = " +
                reader.GetValue(i) ??
                "null");
            Log.Error(ex);
        }

        return info;
    }

    private double DoubleFromCell(int row, int column)
    {
        // row 2 maps to company 0
        var v = workingCapitalData.Companies[row - 2][column];
        if (v == null)
            // Log.Debug($"In DoubleFromCell: {row}, {column} => NULL!");
            return 0;
        if (v is double d)
            // Log.Debug($"In DoubleFromCell: {row}, {column} => {d}");
            return d;
        // Log.Debug($"In DoubleFromCell: {row}, {column} => " + v.GetType().Name);
        return 0;
    }

    private double CalculateDsoUq()
    {
        if (double.IsNaN(dsouq))
        {
            // Log.Info("Calculating DsoUq");
            var values = new double[workingCapitalData.Companies.Count];
            for (var i = 0; i < values.Length; ++i) values[i] = workingCapitalData.Companies[i].Dso;
            // Log.Info("DsoUq " + i + " " + values[i].ToString());
            dsouq = PERCENTILE(values, 0.25);
            // Log.Info("DsoUq = " + dsouq);
        }

        return dsouq;
    }

    private double CalculateDioUq()
    {
        if (double.IsNaN(diouq))
        {
            var values = new double[workingCapitalData.Companies.Count];
            for (var i = 0; i < values.Length; ++i)
                values[i] = workingCapitalData.Companies[i].Dio;
            diouq = PERCENTILE(values, 0.25);
        }

        return diouq;
    }

    private double CalculateDpoUq()
    {
        if (double.IsNaN(dpouq))
        {
            // Log.Info("Calculating DpoUq");
            var values = new double[workingCapitalData.Companies.Count];
            for (var i = 0; i < values.Length; ++i) values[i] = workingCapitalData.Companies[i].Dpo;
            // Log.Info("DpoUq " + i + " " + values[i].ToString());
            dpouq = PERCENTILE(values, 0.75);
            // Log.Info("DpoUq = " + dpouq);
        }

        return dpouq;
    }

    private T IF<T>(bool condition, T ifTrue, T ifFalse)
    {
        return condition ? ifTrue : ifFalse;
    }

    private bool ISERROR(object o)
    {
        return o == null;
    }

    private bool OR(params bool[] conditions)
    {
        foreach (var c in conditions)
        {
            if (c)
                return true;
        }

        return false;
    }

    private double SUM(params double[] items)
    {
        var sum = items[0];
        for (var i = 1; i < items.Length; ++i) sum += items[i];
        return sum;
    }

    private double COUNT(params double[] items)
    {
        return items.Length;
    }

    private double PERCENTRANK(int row1, int row2, int col, int rTarget, int cTarget)
    {
        var v = DoubleFromCell(rTarget, cTarget);
        var d = new double[row2 - row1 + 1];
        for (var i = 0; i < d.Length; ++i)
            d[i] = DoubleFromCell(i + row1, col);
        Array.Sort(d);
        return PERCENTRANK(d, v);
    }

    private static double PERCENTRANK(double[] d, double t)
    {
        int i0 = -1, i1 = -1;
        for (var i = 0; i < d.Length; i++)
        {
            if (d[i] == t)
                return (double)i / (d.Length - 1);
            if (d[i] < t)
            {
                i0 = i;
            }
            else if (d[i] > t)
            {
                i1 = i;
                break;
            }
        }

        if (i0 == -1) return 0;
        if (i1 == -1) return 1;
        double x1 = d[i0], x2 = d[i1];
        var y1 = (double)i0 / (d.Length - 1);
        var y2 = (double)i1 / (d.Length - 1);
        return ((x2 - t) * y1 + (t - x1) * y2) / (x2 - x1);
    }

    private static double PERCENTILE(double[] d, double t)
    {
        Array.Sort(d);
        // for (var i = 0; i < d.Length; i++)
        //     Log.Debug("PERCENTILE [" + i + "]= " + d[i]);

        var a = t * (d.Length - 1); // drop top and bottom
        // Log.Debug("PERCENTILE a=" + a);
        var b1 = (int)a;
        var b2 = b1 + 1;
        // Log.Debug("PERCENTILE b1=" + b1 + " b2=" + b2 + " d[b1]= " + d[b1] + " d[b2]= " + d[b2]);
        var c = a - b1;
        // Log.Debug("PERCENTILE c=" + c);
        if (Math.Abs(c) < 0.00001) // approximately integer
            // Log.Debug("PERCENTILE Returning " + d[b1]);
            return d[b1];

        var e = d[b2] - d[b1];
        // Log.Debug("PERCENTILE e=" + e);
        var f = e * c;
        // Log.Debug("PERCENTILE f=" + f);
        var g = d[b1] + f;
        // Log.Debug("PERCENTILE Returning " + g);
        return g;
    }
}
