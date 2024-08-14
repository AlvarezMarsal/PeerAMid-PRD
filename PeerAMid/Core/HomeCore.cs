using PeerAMid.Business;
using PeerAMid.Data;
using PeerAMid.DataAccess;
using PeerAMid.Support;
using PeerAMid.Utility;
using System.Configuration;
using System.Data;
using System.Linq;

#nullable enable

namespace PeerAMid.Core;

public class HomeCore : IHomeCore
{
    //private static SortedList<int, IndustryModel>? _industries;
    //private static SortedList<int, IndustryModel> Industries => _industries!;
    private static readonly object IndustriesLock = new();
    private static readonly SortedList<int, SortedList<int, SubIndustryModel>> SubIndustries = new();
    private static readonly object SubIndustriesLock = new();
    private static readonly string TargetColor = ConfigurationManager.AppSettings["sgaTargetCompanyColor"];

    //private static string BestInClassColor = ConfigurationManager.AppSettings["sgaBestClassColor"];
    private static readonly string MedianColor = ConfigurationManager.AppSettings["sgaMedianColor"];

    private static readonly string TopQtColor = ConfigurationManager.AppSettings["sgaTopQColor"];

    private readonly IHomeDataAccess _iHomeDataAccess;
    //private static readonly List<Region> _regions = new List<Region>();


    public HomeCore(IHomeDataAccess iHomeDataAccess)
    {
        _iHomeDataAccess = iHomeDataAccess;
    }


    /*
    public List<IndustryModel>? GetIndustryList(string userId)
    {
        if (_industries == null)
        {
            var model = new SortedList<int, IndustryModel>();
            try
            {
                using var iReader = _iHomeDataAccess.GetIndustryList(userId);
                while (iReader?.Read() ?? false)
                {
                    var id = DBDataHelper.GetInt(iReader, "IndustryId");
                    model.Add(id, new IndustryModel(id, DBDataHelper.GetString(iReader, "IndustryName"), true));
                }

                if (_industries != null)
                {
                    lock (_industries)
                    {
                        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                        if (_industries != null)
                        {
                            // Log.Info("Industry list loaded.");
                            _industries = model;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }

        return Industries.Values.ToList();
    }
    */

    public List<IndustryModel> GetSICIndustryCompanyList(string phase, int optionId, int max)
    {
        Log.Info("optionId = " + optionId);
        var model = new List<IndustryModel>();

        try
        {
            using var ireader =
                _iHomeDataAccess.GetSICIndustryCompanyList(
                    phase,
                    optionId == 10 ? 10 : 1,
                    max);
            while (ireader?.Read() ?? false)
            {
                model.Add(new IndustryModel(DBDataHelper.GetInt(ireader, "IndustryId"), DBDataHelper.GetString(ireader, "TextName"), DBDataHelper.GetBoolean(ireader, "IsIndustry")));
            }

            return model;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return model;
        }
    }


    public List<UserCompanyDetails> GetCompanyList(string phrase, int optionID, int max)
    {
        Log.Info("optionId = " + optionID);
        var model = new List<UserCompanyDetails>();
        FillCompanyList(phrase, optionID == 10, max, model);
        return model;
    }


    public UserCompanyDetailsList GetCompanyNames(string phrase, bool byTicker, int max, string? tag = null)
    {
        var model = new UserCompanyDetailsList(tag ?? phrase);
        FillCompanyList(phrase, byTicker, max, model.Companies);
        return model;
    }

    public List<SubIndustryModel>? GetSubIndustryList(int industryId = 0, string? subIndustrySearchText = null)
    {
        var list = new List<SubIndustryModel>();

        try
        {
            lock (SubIndustriesLock)
            {
                if (SubIndustries.Count == 0)
                {
                    using var reader = _iHomeDataAccess.GetSubIndustryList(null, null);
                    while (reader?.Read() ?? false)
                    {
                        var iid = DBDataHelper.GetInt(reader, "IndustryId");
                        var sid = DBDataHelper.GetInt(reader, "SubIndustryId");
                        var m = new SubIndustryModel(iid, sid, DBDataHelper.GetString(reader, "SubIndustryName"));
                        if (!SubIndustries.TryGetValue(iid, out var subIndustries))
                            SubIndustries[iid] = subIndustries = new SortedList<int, SubIndustryModel>();
                        subIndustries[sid] = m;
                    }
                }
                list.Clear();
            }

            if (industryId < 1)
            {
                if (string.IsNullOrEmpty(subIndustrySearchText))
                {
                    foreach (var kvp in SubIndustries)
                        list.AddRange(kvp.Value.Values);
                }
                else
                {
                    foreach (var kvp in SubIndustries)
                    {
                        foreach (var m in kvp.Value)
                        {
                            if (m.Value.SubIndustryName.IndexOf(subIndustrySearchText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                                list.Add(m.Value);
                        }
                    }
                }
            }
            else
            {
                if (SubIndustries.TryGetValue(industryId, out var subindustries))
                {
                    if (string.IsNullOrEmpty(subIndustrySearchText))
                    {
                        list.AddRange(subindustries.Values);
                    }
                    else
                    {
                        foreach (var m in subindustries.Values)
                        {
                            if (m.SubIndustryName.IndexOf(subIndustrySearchText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                                list.Add(m);
                        }
                    }
                }
            }

            return list;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return null;
        }
    }

    public List<PeerCompanyModel> GetPeerCompanyList(string industoryId, string subIndustoryList)
    {
        var model = new List<PeerCompanyModel>();

        try
        {
            using var ireader = _iHomeDataAccess.GetPeerCompanyList(industoryId, subIndustoryList);
            while (ireader.Read())
            {
                model.Add(
                    new PeerCompanyModel
                    {
                        //SubIndustryId = DBDataHelper.GetInt(ireader, "SubIndustryId"),
                        PeerCompanyId = DBDataHelper.GetInt(ireader, "CompanyId"),
                        PeerCompanyName = DBDataHelper.GetString(ireader, "CompanyName"),
                        PeerCompanyDisplayName = DBDataHelper.GetString(ireader, "ShortCompanyMixedCase")
                    });
            }

            return model;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return model;
        }
    }

    public DemographicModel GetDemographicChartData(int targetCompanyId, string selectedPeerList, int year,
        int optionId)
    {
        Log.Info("optionId = " + optionId);
        try
        {
            using var iReader = _iHomeDataAccess.GetDemographicChartData(
                targetCompanyId,
                selectedPeerList,
                year,
                optionId);
            var counter = 0;

            {
                var demographicModel = new DemographicModel();
                // var allOutliers = new SortedDictionary<string, List<string>>();

                while (iReader.Read())
                {
                    counter++;
                    var demoGraphicChartModel = new DemographicChartModel
                    {
                        Minimum = DBDataHelper.GetDecimal(iReader, "MIN"),
                        Percentile25 = DBDataHelper.GetDecimal(iReader, "P25"),
                        Percentile50 = DBDataHelper.GetDecimal(iReader, "P50"),
                        Percentile75 = DBDataHelper.GetDecimal(iReader, "P75"),
                        Maximum = DBDataHelper.GetDecimal(iReader, "MAX"),
                        StarValue = DBDataHelper.GetDecimal(iReader, "TargetValue"),
                        IsCAGRDiv0 = DBDataHelper.IsNull(iReader, "TargetValue")
                    };

                    // Log.Debug("demographicChart[" + counter + "].Minimum      =" + demoGraphicChartModel.Minimum);
                    // Log.Debug("demographicChart[" + counter + "].Percentile25 =" + demoGraphicChartModel.Percentile25);
                    // Log.Debug("demographicChart[" + counter + "].Percentile50 =" + demoGraphicChartModel.Percentile50);
                    // Log.Debug("demographicChart[" + counter + "].Percentile75 =" + demoGraphicChartModel.Percentile75);
                    // Log.Debug("demographicChart[" + counter + "].Maximum      =" + demoGraphicChartModel.Maximum);
                    // Log.Debug("demographicChart[" + counter + "].StarValue    =" + demoGraphicChartModel.StarValue);

                    switch ((DemograpicChartSection)counter)
                    {
                        case DemograpicChartSection.Revenue:
                            demographicModel.RevenueData = demoGraphicChartModel;
                            break;

                        case DemograpicChartSection.CAGR:
                            demographicModel.CAGRData = demoGraphicChartModel;
                            break;

                        case DemograpicChartSection.GrossMargin:
                            demographicModel.GrossMarginData = demoGraphicChartModel;
                            break;

                        case DemograpicChartSection.EBITDA:
                            demographicModel.EBITDAData = demoGraphicChartModel;
                            break;

                        case DemograpicChartSection.NumEmployee:
                            demographicModel.NumEmployeeData = demoGraphicChartModel;
                            break;

                        case DemograpicChartSection.RevenuePerEmployee:
                            demographicModel.RevenuePerEmployeeData = demoGraphicChartModel;
                            break;

                        case DemograpicChartSection.CashConversionCycle:
                            demographicModel.CashConversionCycle = demoGraphicChartModel;
                            break;

                        default:
                            Log.Debug($"Unexpected value for counter {counter}");
                            break;
                    }
                }

                iReader.NextResult();

                // var revLimits = GetOutlierLimits(demographicModel.RevenueData);
                // var cagrLimits = GetOutlierLimits(demographicModel.CAGRData);

                var cccLimits = GetOutlierLimits(demographicModel.CashConversionCycle!);

                // Log.Debug($"Outlier bounds: {cccLimits.Item1}, {cccLimits.Item2}");
                // var ebitdaLimits = GetOutlierLimits(demographicModel.EBITDAData);
                // var marginLimits = GetOutlierLimits(demographicModel.RevenueData);
                // var employeesLimits = GetOutlierLimits(demographicModel.NumEmployeeData);
                // var rpeLimits = GetOutlierLimits(demographicModel.RevenuePerEmployeeData);
                // var cccOutliers = new List<string>();

                while (iReader.Read())
                {
                    var company = DBDataHelper.GetString(iReader, "ShortNameMixedCase");
                    // Log.Debug("Is company outlier? " + company);

                    /*
                        var value = DBDataHelper.GetDecimal(iReader, "CAGR");
                        if (IsOutlier(cagrLimits, value))
                        {
                            if (!allOutliers.TryGetValue(company, out var list))
                                allOutliers.Add(company, list = new List<string>());
                            list.Add("CAGR: " + value);
                        }
                        */

                    var value = DBDataHelper.GetDecimal(iReader, "CCC");
                    // Log.Debug($"checking {value} for outlier-ship against {cccLimits.Item1} and {cccLimits.Item2}");
                    var isOutlier = IsOutlier(cccLimits, value);
                    if (isOutlier)
                    {
                        // Log.Debug($"{company} is outlier!");
                        //var d = Math.Round(value, 0);
                        demographicModel.CashConversionCycle!.Outliers.Add(company);
                    }

                    /*
                        value = DBDataHelper.GetDecimal(iReader, "EBITDA");
                        if (IsOutlier(ebitdaLimits, value))
                        {
                            //if (!allOutliers.TryGetValue(company, out var list))
                            //    allOutliers.Add(company, list = new List<string>());
                            //list.Add("EBITDA: " + value);
                        }

                        value = DBDataHelper.GetDecimal(iReader, "GrossMargin");
                        if (IsOutlier(marginLimits, value))
                        {
                            if (!allOutliers.TryGetValue(company, out var list))
                                allOutliers.Add(company, list = new List<string>());
                            list.Add("Gross Margin: " + value);
                        }

                        value = DBDataHelper.GetDecimal(iReader, "NoOfEmployees");
                        if (IsOutlier(employeesLimits, value))
                        {
                            if (!allOutliers.TryGetValue(company, out var list))
                                allOutliers.Add(company, list = new List<string>());
                            list.Add("Employees: " + value);
                        }

                        value = DBDataHelper.GetDecimal(iReader, "Revenue");
                        if (IsOutlier(revLimits, value))
                        {
                            if (!allOutliers.TryGetValue(company, out var list))
                                allOutliers.Add(company, list = new List<string>());
                            list.Add("Revenue: " + value);
                        }

                        value = DBDataHelper.GetDecimal(iReader, "RevPerEE");
                        if (IsOutlier(rpeLimits, value))
                        {
                            if (!allOutliers.TryGetValue(company, out var list))
                                allOutliers.Add(company, list = new List<string>());
                            list.Add("Revenue per Employee: $" + (int)value);
                        }
                        */
                }

                var items = demographicModel.CashConversionCycle!.Outliers;
                demographicModel.Outliers = StringExtensionMethods.OxfordComma(items);
                if (items.Count == 1)
                    demographicModel.Outliers += " is an outlier.";
                else if (items.Count > 1) demographicModel.Outliers += " are outliers.";
                if (items.Count > 0)
                    demographicModel.Outliers += "  Users are advised to omit outliers, as there may be issues with reported data.";

                // Log.Debug("Outliers " + demographicModel.Outliers);
                Log.Debug("demographicModel " + demographicModel);
                return demographicModel;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return new DemographicModel();
        }

        (decimal, decimal) GetOutlierLimits(DemographicChartModel dcm)
        {
            var gap = (dcm.Percentile75 - dcm.Percentile25) * (decimal)1.5;
            return (dcm.Percentile25 - gap, dcm.Percentile75 + gap);
        }

        bool IsOutlier((decimal, decimal) limits, decimal value)
        {
            return value < limits.Item1 || value > limits.Item2;
        }
    }

    public List<int> GetFutureYearList()
    {
        var model = new List<int>();

        try
        {
            using var ireader = _iHomeDataAccess.GetFutureYears();
            while (ireader.Read())
            {
                model.Add(DBDataHelper.GetInt(ireader, "FsYear"));
            }

            return model;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return model;
        }
    }

    public SGAModel GetSgaChartData(int targetCompanyId, string targetCompanySymbol, string selectedPeerList, int year,
        int optionId, bool requireMatchingFiscalYear = false)
    {
        Log.Info("optionId = " + optionId);
        try
        {
            var model = new SGAModel
            {
                ChartData = new ChartModel(),
                SelectedData = GetSelectedTargetAndSubIndustriesModel(targetCompanySymbol, selectedPeerList)
            };

            using var iReader = _iHomeDataAccess.GetSGAChartData(
                targetCompanyId,
                selectedPeerList,
                year,
                optionId,
                requireMatchingFiscalYear);

            var chartParamModel = new ChartParamModel(false);

            if (iReader.Read())
            {
                // string commonColorCode = ConfigurationManager.AppSettings["sgaCommonColor"];

                chartParamModel.P10Value = DBDataHelper.GetDecimal(iReader, "P10");
                chartParamModel.P25Value = DBDataHelper.GetDecimal(iReader, "P25");
                chartParamModel.P50Value = DBDataHelper.GetDecimal(iReader, "P50");
                chartParamModel.P75Value = DBDataHelper.GetDecimal(iReader, "P75");
                chartParamModel.P90Value = DBDataHelper.GetDecimal(iReader, "P90");
                chartParamModel.TargetValue = DBDataHelper.GetDecimal(iReader, "PTarget");
                chartParamModel.TargetRevenue = DBDataHelper.GetDecimal(iReader, "RevTarget");

                iReader.NextResult();

                model.ChartData.ProviderData = new List<ChartProviderModel>();

                while (iReader.Read())
                {
                    var cpm = new ChartProviderModel
                    {
                        UID = DBDataHelper.GetInt(iReader, "UID"),
                        PeerCompanyValue = DBDataHelper.GetDecimal(iReader, "SGA"),
                        PeerCompanyName = DBDataHelper.GetString(iReader, "CompanyName").ToUpper(),
                        PeerCompanyDisplayName = DBDataHelper.GetString(iReader, "ShortNameMixedCase"),
                        //Color = commonColorCode,
                        //ColorFirst = commonColorCode,
                        IsTarget = DBDataHelper.GetInt(iReader, "UID") == targetCompanyId
                    };

                    // Log.Info("Got display name as " + cpm.PeerCompanyDisplayName);

                    model.ChartData.ProviderData.Add(cpm);

                    // Log.Debug(cpm.PeerCompanyName);
                }

                model.DifferenceTableData = SetSGADifferenceTableData(
                    chartParamModel,
                    model.ChartData.ProviderData,
                    chartParamModel.TargetRevenue * 1000);
                FormatChartWithColorCode(model.ChartData.ProviderData /*, chartParamModel*/);
                return model;
            }

            return model;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return new SGAModel();
        }
    }

    public WorkingCapitalData GetWorkingCapitalData(int targetCompanyId, string targetCompanySymbol,
        string selectedPeerList, int year, int optionId,
        bool requireMatchingFiscalYear = false)
    {
        Log.Info("optionId = " + optionId);
        try
        {
            var selectedData = GetSelectedTargetAndSubIndustriesModel(targetCompanySymbol, selectedPeerList);

            var reader = _iHomeDataAccess.GetWorkingCapitalData(
                targetCompanyId,
                selectedPeerList,
                year,
                optionId,
                requireMatchingFiscalYear);

            var data = new WorkingCapitalData(selectedData!);
            using (reader)
            {
                while (reader.Read())
                {
                    DBDataHelper.Get(reader, "UID", out int uid);
                    var info = data.FindCompany(uid);
                    if (info == null)
                    {
                        info = CompanyInfo.CreateFromDataReader(data, data.Companies.Count, reader, uid);
                        if (info != null)
                            data.Companies.Add(info);
                        // Log.Debug("Added company " + info.Name);
                    }
                    else
                    {
                        CompanyInfo.UpdateFromDataReader(info, reader);
                        // Log.Debug("Updated company " + info.Name);
                    }
                }

                CompanyInfo.NumberOfCompanies = data.Companies.Count;

                while (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        DBDataHelper.Get(reader, "Name", out string name);
                        DBDataHelper.Get(reader, "Inverse", out bool isInverse);
                        DBDataHelper.Get(reader, "P000", out double p000);
                        DBDataHelper.Get(reader, "P010", out double p010);
                        DBDataHelper.Get(reader, "P025", out double p025);
                        DBDataHelper.Get(reader, "P050", out double p050);
                        DBDataHelper.Get(reader, "P075", out double p075);
                        DBDataHelper.Get(reader, "P090", out double p090);
                        DBDataHelper.Get(reader, "P100", out double p100);
                        var summary = new SummaryData(
                            name,
                            isInverse,
                            p000,
                            p010,
                            p025,
                            p050,
                            p075,
                            p090,
                            p100,
                            data.Companies);
                        data.SummaryData.Add(name, summary);
                        // Log.Debug("Added collection " + name);
                    }
                }

                data.Companies.Sort((a, b) => a.Name.CompareTo(b.Name));

                // Log.Info(data.ToString());
                // Log.Debug("LEAVING");
                return data;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            // Log.Debug($"LEAVING GetWorkingCapitalData");
            return new WorkingCapitalData(new SelectedTargetAndSubIndustriesModel());
        }
    }

    public WorkingCapitalTrendData GetWorkingCapitalTrendData(int targetCompanyId, string selectedPeerList,
        int firstYear, int lastYear, int maxYears)
    {
        try
        {
            var reader = _iHomeDataAccess.GetWorkingCapitalTrendData(
                targetCompanyId,
                selectedPeerList,
                firstYear,
                lastYear);
            if (reader == null)
                throw new Exception("Read failed");

            var data = new WorkingCapitalTrendData(firstYear, lastYear);
            using (reader)
            {
                var first = int.MaxValue;
                var last = int.MinValue;

                do
                {
                    while (reader.Read())
                    {
                        DBDataHelper.Get(reader, "Name", out string name);
                        DBDataHelper.Get(reader, "Year", out int year);
                        DBDataHelper.Get(reader, "P050", out double value);

                        if (year < first)
                            first = year;
                        else if (year > last)
                            last = year;

                        var annualData = data.AnnualData[year];
                        switch (name)
                        {
                            case "CCC":
                                annualData.CCC = value;
                                break;

                            case "DIO":
                                annualData.DIO = value;
                                break;

                            case "DPO":
                                annualData.DPO = value;
                                break;

                            case "DSO":
                                annualData.DSO = value;
                                break;

                            default:
                                Log.Error("Unexpected " + name);
                                break;
                        }
                    }
                } while (reader.NextResult());

                // Remove years at the ends with no data
                var keys = data.AnnualData.Keys.ToArray();
                foreach (var key in keys)
                {
                    if (key < first || key > last)
                        data.AnnualData.Remove(key);
                }

                while (data.AnnualData.Count > maxYears) data.AnnualData.RemoveAt(0);

                // Log.Info(data.ToString());

                return data;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            // Log.Debug($"LEAVING GetWorkingCapitalData");
            return new WorkingCapitalTrendData(0, 0);
        }
    }

    public SGAPerformanceModel GetPerformanceChartData(int targetCompanyId, string selectedPeerList, int year,
        int optionId)
    {
        Log.Info("optionId = " + optionId);
        var model = new SGAPerformanceModel
        {
            SGAPerformanceData = new List<SGAPerformaceItemModel>()
        };

        try
        {
            //var textInfo = new CultureInfo("en-US", false).TextInfo;
            using var ireader = _iHomeDataAccess.GetSGAPerformanceChartData(
                targetCompanyId,
                selectedPeerList,
                year,
                optionId);
            model.SGAPerformanceData = new List<SGAPerformaceItemModel>();

            var performanceTargetColor = ConfigurationManager.AppSettings["performanceTargetColor"];
            var performancePeerColor = ConfigurationManager.AppSettings["performancePeerColor"];

            SGAPerformaceItemModel sgaData;
            bool isTarget;
            decimal xValue;
            decimal yValue;

            while (ireader != null && ireader.Read())
            {
                sgaData = new SGAPerformaceItemModel();
                isTarget = DBDataHelper.GetBoolean(ireader, "IsTarget");
                sgaData.IsTarget = isTarget;
                sgaData.ColorCode = isTarget ? performanceTargetColor : performancePeerColor;

                xValue = DBDataHelper.GetDecimal(ireader, "XAxis");
                yValue = DBDataHelper.GetDecimal(ireader, "YAxis");
                sgaData.XValue = xValue;
                sgaData.YValue = yValue;
                sgaData.X2Value = xValue;
                sgaData.Y2Value = yValue;
                sgaData.PeerCompanyValue = DBDataHelper.GetDecimal(ireader, "EBITDA Margin");
                sgaData.EBITDAMarginValue = DBDataHelper.GetDecimal(ireader, "EBITDA Margin");
                sgaData.SGAMarginValue = DBDataHelper.GetDecimal(ireader, "SGA Margin");
                sgaData.PeerCompanyName = DBDataHelper.GetString(ireader, "CompanyName").ToUpper();
                sgaData.PeerCompanyDisplayName = DBDataHelper.GetString(ireader, "ShortNameMixedCase");
                // Log.Info("sgaData.PeerCompanyDisplayName = " + sgaData.PeerCompanyDisplayName);
                model.SGAPerformanceData.Add(sgaData);
            }

            return model;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return model;
        }
    }

    public FunctionalRevenueModel GetFunCostAsPercentOfRevenueData(int targetCompanyId, string selectedPeerList,
        int year, int optionId)
    {
        Log.Info("optionId = " + optionId);
        try
        {
            using var ireader = _iHomeDataAccess.GetFunCostAsPercentOfRevenueData(
                targetCompanyId,
                selectedPeerList,
                year,
                optionId);
            var model = new FunctionalRevenueModel
            {
                FinanceData = GetChart4Data(ireader),
                HRData = GetChart4Data(ireader),
                ITData = GetChart4Data(ireader),
                ProcurementData = GetChart4Data(ireader),
                SalesData = GetChart4Data(ireader),
                MarketData = GetChart4Data(ireader),
                CustServData = GetChart4Data(ireader),
                CSSupportServData = GetChart4Data(ireader)
            };

            return model;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return new FunctionalRevenueModel();
        }
    }

    public FunctionalRevenueModel GetFTEDecomposedCostAsPercentOfRevenueData(
        int targetCompanyId, string selectedPeerList, int year, int optionId)
    {
        Log.Info("optionId = " + optionId);
        try
        {
            using var ireader = _iHomeDataAccess.GetFTEDecomposedCostAsPercentOfRevenueData(
                targetCompanyId,
                selectedPeerList,
                year,
                optionId);
            var model = new FunctionalRevenueModel
            {
                FinanceData = GetChart5Data(ireader),
                HRData = GetChart5Data(ireader),
                ITData = GetChart5Data(ireader),
                ProcurementData = GetChart5Data(ireader),
                SalesData = GetChart5Data(ireader),
                MarketData = GetChart5Data(ireader),
                CustServData = GetChart5Data(ireader),
                CSSupportServData = GetChart5Data(ireader)
            };

            model.FTESummaryLine = GetFTESummaryLogic(model);

            return model;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return new FunctionalRevenueModel();
        }
    }

    public List<Company> GetFinalPeerAndTargetCompaniesList(int targetCompanyId, string selectedPeerList, int year)
    {
        try
        {
            using var ireader = _iHomeDataAccess.GetFinalPeerAndTargetCompaniesList(targetCompanyId, selectedPeerList, year);
            return PeerAMidCore.ParseCompanies(ireader);
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return new List<Company>();
        }
    }

    public SGAWaterfallModel GetDecomposedWaterFallTopQ(int targetCompanyId, string selectedPeerList, int year,
        int optionId)
    {
        Log.Info("optionId = " + optionId);
        try
        {
            using var ireader = _iHomeDataAccess.GetDecomposedSGAWaterFallTopQ(
                targetCompanyId,
                selectedPeerList,
                year,
                optionId);
            var model = new SGAWaterfallModel();
            var lstwaterfalldata = new List<SGAWaterfallItemModel>();

            while (ireader.Read())
            {
                lstwaterfalldata.Add(
                    new SGAWaterfallItemModel
                    {
                        DepartmentName = DBDataHelper.GetString(ireader, "SectionName"),
                        DepartmentValue = DBDataHelper.GetDecimal(ireader, "Target vs P25 $"),
                        StartValue = DBDataHelper.GetDecimal(ireader, "StartIndex"),
                        EndValue = DBDataHelper.GetDecimal(ireader, "EndIndex")
                    });
            }

            var lastValue = lstwaterfalldata.Last().EndValue;
            model.WaterfallChartItemList = ConvertFormatedModel(lstwaterfalldata, lastValue);
            decimal startValue = 0;
            lstwaterfalldata.Clear();
            foreach (var data in model.WaterfallChartItemList)
            {
                lstwaterfalldata.Add(
                    new SGAWaterfallItemModel
                    {
                        DepartmentName = data.DepartmentName,
                        DepartmentValue = data.DepartmentValue,
                        StartValue = startValue,
                        EndValue = startValue + data.DepartmentValue
                    });
                startValue += data.DepartmentValue;
            }

            model.WaterfallChartItemList = lstwaterfalldata;
            lastValue = model.WaterfallChartItemList.Last().EndValue;
            model.WaterfallChartItemList.Add(
                new SGAWaterfallItemModel
                {
                    StartValue = 0,
                    EndValue = lastValue,
                    DepartmentName = "SG&A",
                    DepartmentValue = lastValue
                });
            return model;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return new SGAWaterfallModel();
        }
    }

    public SGAWaterfallModel GetDecomposedWaterFallMedian(int targetCompanyId, string selectedPeerList, int year,
        int optionId)
    {
        Log.Info("optionId = " + optionId);
        try
        {
            using var ireader = _iHomeDataAccess.GetDecomposedSGAWaterFallMedian(
                targetCompanyId,
                selectedPeerList,
                year,
                optionId);
            var model = new SGAWaterfallModel();
            var lstwaterfalldata = new List<SGAWaterfallItemModel>();

            while (ireader.Read())
            {
                lstwaterfalldata.Add(
                    new SGAWaterfallItemModel
                    {
                        DepartmentName = DBDataHelper.GetString(ireader, "SectionName"),
                        DepartmentValue = DBDataHelper.GetDecimal(ireader, "Target vs P50 $"),
                        StartValue = DBDataHelper.GetDecimal(ireader, "StartIndex"),
                        EndValue = DBDataHelper.GetDecimal(ireader, "EndIndex")
                    });
            }

            var lastValue = lstwaterfalldata.Last().EndValue;
            model.WaterfallChartItemList = ConvertFormatedModel(lstwaterfalldata, lastValue);
            decimal startValue = 0;
            lstwaterfalldata.Clear();
            foreach (var data in model.WaterfallChartItemList)
            {
                lstwaterfalldata.Add(
                    new SGAWaterfallItemModel
                    {
                        DepartmentName = data.DepartmentName,
                        DepartmentValue = data.DepartmentValue,
                        StartValue = startValue,
                        EndValue = startValue + data.DepartmentValue
                    });
                startValue += data.DepartmentValue;
            }

            model.WaterfallChartItemList = lstwaterfalldata;
            lastValue = model.WaterfallChartItemList.Last().EndValue;
            model.WaterfallChartItemList.Add(
                new SGAWaterfallItemModel
                {
                    StartValue = 0,
                    EndValue = lastValue,
                    DepartmentName = "SG&A",
                    DepartmentValue = lastValue
                });

            return model;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return new SGAWaterfallModel();
        }
    }

    public SGAWaterfallModel GetDecomposedWaterFallTopD(int targetCompanyId, string selectedPeerList, int year,
        int optionId)
    {
        Log.Info("optionId = " + optionId);
        try
        {
            using var ireader = _iHomeDataAccess.GetDecomposedSGAWaterFallTopD(
                targetCompanyId,
                selectedPeerList,
                year,
                optionId);
            var model = new SGAWaterfallModel();
            model.TargetCompName = SessionData.Instance.BenchmarkCompany.Name;
            var lstwaterfalldata = new List<SGAWaterfallItemModel>();

            while (ireader.Read())
            {
                lstwaterfalldata.Add(
                    new SGAWaterfallItemModel
                    {
                        DepartmentName = DBDataHelper.GetString(ireader, "SectionName"),
                        DepartmentValue = DBDataHelper.GetDecimal(ireader, "Target vs P10 $"),
                        StartValue = DBDataHelper.GetDecimal(ireader, "StartIndex"),
                        EndValue = DBDataHelper.GetDecimal(ireader, "EndIndex")
                    });
            }

            var lastValue = lstwaterfalldata.Last().EndValue;
            model.WaterfallChartItemList = ConvertFormatedModel(lstwaterfalldata, lastValue);
            decimal startValue = 0;
            lstwaterfalldata.Clear();
            foreach (var data in model.WaterfallChartItemList)
            {
                lstwaterfalldata.Add(
                    new SGAWaterfallItemModel
                    {
                        DepartmentName = data.DepartmentName,
                        DepartmentValue = data.DepartmentValue,
                        StartValue = startValue,
                        EndValue = startValue + data.DepartmentValue
                    });
                startValue += data.DepartmentValue;
            }

            model.WaterfallChartItemList = lstwaterfalldata;
            lastValue = model.WaterfallChartItemList.Last().EndValue;
            model.WaterfallChartItemList.Add(
                new SGAWaterfallItemModel
                {
                    StartValue = 0,
                    EndValue = lastValue,
                    DepartmentName = "SG&A",
                    DepartmentValue = lastValue
                });

            Log.Debug(model.ToString());
            return model;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return new SGAWaterfallModel();
        }
    }

    public AppPhrases GetAppPhrases(AppPhrases? currentAppPhrases, string app, string? language)
    {
        if ((currentAppPhrases == null) ||
           ((app != null) && (currentAppPhrases.App != app)) ||
           ((language != null) && (currentAppPhrases.Language != language)))
        {
            currentAppPhrases = new AppPhrases(app!, language!)
            {
                Version = -1
            };
        }

        try
        {
            var reader = _iHomeDataAccess.GetAppPhrases(
                currentAppPhrases.App,
                currentAppPhrases.Language,
                currentAppPhrases.Version);
            if (reader == null)
                throw new Exception("Read failed");

            using (reader)
            {
                // The first SELECT is just the new version number
                if (reader.Read())
                {
                    DBDataHelper.Get(reader, "CurrentVersion", out int d);
                    currentAppPhrases.Version = d;

                    // Then, up to two languages: the default language and the selected language
                    while (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            DBDataHelper.Get(reader, "LogicId", out int id);
                            DBDataHelper.Get(reader, "Subject", out string subject);
                            DBDataHelper.Get(reader, "Topic", out string topic);
                            DBDataHelper.Get(reader, "Condition", out string condition);
                            DBDataHelper.Get(reader, "Text", out string text);
                            currentAppPhrases.AddPhrase(subject, topic, id, condition, text);
                        }
                    }
                }
            }

            return currentAppPhrases;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            // Log.Debug($"LEAVING GetWorkingCapitalData");
            return currentAppPhrases;
        }
    }


    private void FillCompanyList(string phrase, bool byTicker, int max, IList<UserCompanyDetails> model)
    {
        var byCompanyName = !byTicker;
        try
        {
            var previousKey = "";
            using var iReader = _iHomeDataAccess.GetSICIndustryCompanyList(phrase, byTicker ? 10 : 0, max);
            while (iReader != null && iReader.Read())
            {
                var companyId = DBDataHelper.GetInt(iReader, "UID");
                var companyName = DBDataHelper.GetString(iReader, "coname");
                var dataYear = DBDataHelper.GetInt(iReader, "DataYear");
                var ticker = DBDataHelper.GetString(iReader, "Ticker");
                var key = byCompanyName ? companyName : ticker;
                Log.Debug($"{companyId} '{companyName}' '{ticker}' {dataYear} '{key}'");
                if (key != previousKey)
                {
                    previousKey = key;
                    model.Add(
                        new UserCompanyDetails
                        {
                            CompanyID = companyId,
                            CompanyName = companyName,
                            DataYear = dataYear,
                            Ticker = ticker,
                            DisplayText = byCompanyName ? companyName : ticker + "   " + companyName
                        });
                    ;
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            model.Clear();
        }
    }

    // ReSharper disable once InconsistentNaming
    private DifferenceTableModel<decimal>? SetSGADifferenceTableData(ChartParamModel chartParamModel,
        List<ChartProviderModel> providers,
        decimal opportunityMultiplier)
    {
        try
        {
            // Log.Info(
            //    $"chartParamModel.P10Value = {chartParamModel.P10Value}\n" +
            //    $"chartParamModel.P25Value = {chartParamModel.P25Value}\n" +
            //    $"chartParamModel.P50Value = {chartParamModel.P50Value}\n" +
            //    $"chartParamModel.P75Value = {chartParamModel.P75Value}\n" +
            //    $"chartParamModel.P90Value = {chartParamModel.P90Value}\n" +
            //    $"chartParamModel.TargetValue   = {chartParamModel.TargetValue}\n" +
            //    $"chartParamModel.TargetRevenue = {chartParamModel.TargetRevenue}\n" +
            //    $"opportunityMultiplier = {opportunityMultiplier}");

            var model = new DifferenceTableModel<decimal>(false)
            {
                CostTopDecile = chartParamModel.P10Value,
                CostTopQuartile = chartParamModel.P25Value,
                CostMedian = chartParamModel.P50Value,
                CostBottomQuartile = chartParamModel.P75Value,
                CostBottomDecile = chartParamModel.P90Value,
                CostTarget = chartParamModel.TargetValue
            };

            var targetIndex = 0;
            var max = providers[0].PeerCompanyValue;
            var min = providers[providers.Count - 1].PeerCompanyValue;
            for (var i = 0; i < providers.Count; ++i)
            {
                if (providers[i].IsTarget)
                    targetIndex = i;
            }

            model.CostMin = model.InvertedValues ? max : min;
            model.CostMax = model.InvertedValues ? min : max;

            // With Min and Max calculated, we can determine rankings
            for (var i = 0; i < providers.Count; ++i)
                providers[i].Ranking = model.DetermineRanking(providers[i].PeerCompanyValue /*, providers[i].IsTarget*/);

            // hack: we don't want the median to fall between two companies.  So if there's an
            // even number of companies, we make there be two medians
            var medianIndex = providers.Count / 2;
            // Log.Debug("Median index:" + medianIndex);
            providers[medianIndex].IsMedian = true;
            if (providers.Count % 2 == 0)
                providers[medianIndex - 1].IsMedian = true;

            model.TargetRanking = providers[targetIndex].Ranking;

            // Log.Info(model.ToString());
            // for (var i = 0; i < providers.Count; ++i)
            //     Log.Info($"Provider[{i}] = {providers[i].PeerCompanyName} => {providers[i].PeerCompanyValue}, {providers[i].PeerCompanyDisplayName}");

            if (chartParamModel.P10Value == 0)
            {
                model.CostGapTopDecile = 0;
            }
            else
            {
                model.CostGapTopDecile = Math.Round(
                    (chartParamModel.TargetValue / chartParamModel.P10Value - 1) * 100,
                    2);
            }

            if (chartParamModel.P25Value == 0)
            {
                model.CostGapTopQuartile = 0;
            }
            else
            {
                model.CostGapTopQuartile = Math.Round(
                    (chartParamModel.TargetValue / chartParamModel.P25Value - 1) * 100,
                    2);
            }

            if (chartParamModel.P50Value == 0)
            {
                model.CostGapMedian = 0;
            }
            else
            {
                model.CostGapMedian = Math.Round(
                    (chartParamModel.TargetValue / chartParamModel.P50Value - 1) * 100,
                    2);
            }

            model.CostGapTarget = 0;

            model.OpportunityTopDecile = Math.Round(
                (chartParamModel.TargetValue - chartParamModel.P10Value) / 100 * opportunityMultiplier,
                2);
            model.OpportunityTopQuartile = Math.Round(
                (chartParamModel.TargetValue - chartParamModel.P25Value) / 100 * opportunityMultiplier,
                2);
            model.OpportunityMedian = Math.Round(
                (chartParamModel.TargetValue - chartParamModel.P50Value) / 100 * opportunityMultiplier,
                2);

            var gap = (chartParamModel.P75Value - chartParamModel.P25Value) * (decimal)1.5;
            var lowerOutlierLimit = chartParamModel.P25Value - gap;
            var upperOutlierLimit = chartParamModel.P75Value + gap;

            Log.Debug("LowerOutlierLimit: " + lowerOutlierLimit);
            Log.Debug("UpperOutlierLimit: " + upperOutlierLimit);

            var outliers = new List<string>();
            foreach (var sc in providers)
            {
                var isOutlier = sc.PeerCompanyValue < lowerOutlierLimit || sc.PeerCompanyValue > upperOutlierLimit;
                Log.Debug(sc.PeerCompanyDisplayName + ": " + sc.PeerCompanyValue + " " + isOutlier);
                if (isOutlier)
                {
                    sc.IsOutlier = true;
                    outliers.Add(sc.PeerCompanyDisplayName + " (" + sc.PeerCompanyValue.ToString("0.00") + "%)");
                }
            }

            var o = StringExtensionMethods.OxfordComma(outliers);
            if (outliers.Count == 1)
                o += " is an outlier.";
            else if (outliers.Count > 1) o += " are outliers.";
            if (outliers.Count > 0)
                o += "  Users are advised to omit outliers, as there may be issues with reported data.";
            model.OutliersDescription = o;

            // Log.Debug($"model.OutliersDescription = '{model.OutliersDescription}'");

            return model;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }

        return null;
    }

    private static void FormatChartWithColorCode(List<ChartProviderModel> sgaChartData /*, ChartParamModel paramModel*/)
    {
        try
        {
            /*
            var sgaTargetCompanyColor = ConfigurationManager.AppSettings["sgaTargetCompanyColor"];
            var sgaMedianColor = ConfigurationManager.AppSettings["sgaMedianColor"];
            var sgaBottomQuartileColor = ConfigurationManager.AppSettings["sgaBottomQuartileColor"];
            */
            if (sgaChartData != null && sgaChartData.Count > 0 /* && paramModel != null)*/)
            {
                var itemCount = sgaChartData.Count;
                var median1 = itemCount / 2;
                var median2 = median1 - 1 + itemCount % 2; // for odd numbers, median2 == median1; for even numbers, median2 = median1 - 1

                //Setting Color for Median
                sgaChartData[median1].IsMedian = true;
                sgaChartData[median1].Ranking = Ranking.Median;
                if (median2 != median1)
                {
                    sgaChartData[median2].IsMedian = true;
                    sgaChartData[median2].Ranking = Ranking.Median;
                }
                // Log.Debug($"Median1 = {median1}, Median2= {median2}");

                //Setting Color for Target
                /*var target = sgaChartData.FindIndex(s => s.IsTarget);
                sgaChartData[target].Color = sgaTargetCompanyColor;
                if ((target != median1) && (target != median2))
                {
                    sgaChartData[target].ColorFirst = sgaTargetCompanyColor;
                }
                */
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
    }

    public SelectedTargetAndSubIndustriesModel? GetSelectedTargetAndSubIndustriesModel(
        string targetCompanySymbol, string subIndustoryList)
    {
        var model = new SelectedTargetAndSubIndustriesModel
        {
            SubIndustryNames = new List<string>(),
            TargetSymbol = targetCompanySymbol
        };

        try
        {
            using var ireader = _iHomeDataAccess.GetSelectedTargetAndSubIndustriesModel(subIndustoryList);
            var isFrist = true;

            while (ireader != null && ireader.Read())
            {
                if (isFrist)
                {
                    model.IndustryName = DBDataHelper.GetString(ireader, "Industry");
                    isFrist = false;
                }

                model.SubIndustryNames.Add(DBDataHelper.GetString(ireader, "SubIndustry"));
            }

            return model;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return null;
        }
    }

    public List<FunctionalRevenueItemModel> GetChart4Data(IDataReader ireader)
    {
        if (ireader.Read())
        {
            return new List<FunctionalRevenueItemModel>
                {
                    new()
                    {
                        Color = TargetColor,
                        PeerCompanyName = "Target",
                        PeerCompanyValue = DBDataHelper.GetDecimal(ireader, "TargetValue"),
                        RankID = 2
                    },
                    new()
                    {
                        Color = TopQtColor,
                        PeerCompanyName = "Top Quartile",
                        PeerCompanyValue = DBDataHelper.GetDecimal(ireader, "P25"),
                        RankID = 3
                    },
                    new()
                    {
                        Color = MedianColor,
                        PeerCompanyName = "Median",
                        PeerCompanyValue = DBDataHelper.GetDecimal(ireader, "P50"),
                        RankID = 1
                    }
                }.OrderByDescending(val => val.PeerCompanyValue)
                .ThenBy(n => n.RankID)
                .ToList();
        }

        return new List<FunctionalRevenueItemModel>();
    }

    public List<FunctionalRevenueItemModel> GetChart5Data(IDataReader ireader)
    {
        if (ireader.Read())
        {
            return new List<FunctionalRevenueItemModel>
                {
                    new()
                    {
                        Color = TargetColor,
                        IsTarget = true,
                        PeerCompanyName = "Target",
                        PeerCompanyValue = DBDataHelper.GetDecimal(ireader, "TargetValue"),
                        RankID = 2
                    },
                    new()
                    {
                        Color = TopQtColor,
                        PeerCompanyName = "Top Quartile",
                        PeerCompanyValue = DBDataHelper.GetDecimal(ireader, "P25"),
                        RankID = 3
                    },
                    new()
                    {
                        Color = MedianColor,
                        PeerCompanyName = "Median",
                        PeerCompanyValue = DBDataHelper.GetDecimal(ireader, "P50"),
                        RankID = 1
                    }
                }.OrderByDescending(val => val.PeerCompanyValue)
                .ThenBy(n => n.RankID)
                .ToList();
        }

        return new List<FunctionalRevenueItemModel>();
    }

    private string GetFTESummaryLogic(FunctionalRevenueModel model)
    {
        var summaryLine = "";

        try
        {
            if (model != null)
            {
                if (model.CSSupportServData[0].IsTarget ||
                    model.CustServData[0].IsTarget ||
                    model.FinanceData[0].IsTarget ||
                    model.HRData[0].IsTarget ||
                    model.ITData[0].IsTarget ||
                    model.MarketData[0].IsTarget ||
                    model.ProcurementData[0].IsTarget ||
                    model.SalesData[0].IsTarget
                   )
                {
                    // case 2
                    summaryLine =
                        "Decomposed FTEs per billion of revenue indicates that ##CompanyName## is performing below median in the back office functions.";
                }
                else if (model.CSSupportServData[0].IsTarget != true &&
                         model.CustServData[0].IsTarget != true &&
                         model.FinanceData[0].IsTarget != true &&
                         model.HRData[0].IsTarget != true &&
                         model.ITData[0].IsTarget != true &&
                         model.MarketData[0].IsTarget != true &&
                         model.ProcurementData[0].IsTarget != true &&
                         model.SalesData[0].IsTarget != true &&
                         (model.CSSupportServData[1].IsTarget ||
                          model.CustServData[1].IsTarget ||
                          model.FinanceData[1].IsTarget ||
                          model.HRData[1].IsTarget ||
                          model.ITData[1].IsTarget ||
                          model.MarketData[1].IsTarget ||
                          model.ProcurementData[1].IsTarget ||
                          model.SalesData[1].IsTarget
                         ))
                {
                    //case 1
                    summaryLine =
                        "Decomposed FTEs per billion of revenue indicates that ##CompanyName## is performing better than median but below than top quartile performers in the back office functions.";
                }
                else
                {
                    //case 3
                    summaryLine =
                        "Decomposed FTEs per billion of revenue indicates that ##CompanyName## is performing better than top quartile performers in the back office functions.";
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return string.Empty;
        }

        return summaryLine;
    }

    private List<SGAWaterfallItemModel> ConvertFormatedModel(List<SGAWaterfallItemModel> waterfallItems,
        decimal lastValue)
    {
        var decimalPlace = 0;

        if (lastValue >= 0 && lastValue <= 1)
            //3 decimal
            decimalPlace = 3;
        else if (lastValue > 1 && lastValue <= 5)
            // 2 decimal
            decimalPlace = 2;
        else if (lastValue > 5 && lastValue <= 10)
            // 1 decimal
            decimalPlace = 1;
        else
            // 0 decimal
            decimalPlace = 0;

        var formattedData = new List<SGAWaterfallItemModel>();
        foreach (var w in waterfallItems)
        {
            var a = new SGAWaterfallItemModel
            {
                DepartmentName = w.DepartmentName,
                DepartmentValue = decimal.Round(w.DepartmentValue, decimalPlace, MidpointRounding.AwayFromZero)
                //EndValue /*= w.EndValue*/= decimal.Round(w.EndValue, decimalPlace, MidpointRounding.AwayFromZero),
                //StartValue /*= w.StartValue*/= decimal.Round(w.StartValue, decimalPlace, MidpointRounding.AwayFromZero)
            };
            formattedData.Add(a);
        }

        return formattedData;
    }
}
