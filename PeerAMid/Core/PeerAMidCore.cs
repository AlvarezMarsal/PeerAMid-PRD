using PeerAMid.Business;
using PeerAMid.Data;
using PeerAMid.DataAccess;
using PeerAMid.Support;
using PeerAMid.Utility;
using System.Data;

#nullable enable 

namespace PeerAMid.Core;

public class PeerAMidCore : IPeerAMidCore
{
    private readonly IPeerAMidDataAccess _iPeerAMidDataAccess;

    public PeerAMidCore(IPeerAMidDataAccess iPeerAMidDataAccess)
    {
        _iPeerAMidDataAccess = iPeerAMidDataAccess;
    }

    public List<Company> GetBenchmarkCompanyList(string cSearch = "", string tSearch = "",
        string? regions = "", string year = "0")
    {
        using var reader = _iPeerAMidDataAccess.GetBenchmarkCompanyList(
            cSearch,
            tSearch,
            regions,
            year);
        var requireWorkingCapitalData = SessionData.Instance.SelectedService == PeerAMidService.WcdFull || SessionData.Instance.SelectedService == PeerAMidService.WcdShort;
        return ProcessBenchmarkCompanyList(reader, requireWorkingCapitalData);
    }

    public Company? GetCompanyDetails(int companyId, int? year)
    {
        return GetCompanyDetails(companyId.ToString(), year?.ToString());
    }

    public Company? GetCompanyDetails(string companyId, string? year = null)
    {
        using var reader = _iPeerAMidDataAccess.GetCompanyDetails(companyId, year);
        if (reader?.Read() ?? false)
        {
            var company = ParseCompany(reader);
            // Log.Debug(JsonConvert.SerializeObject(company));
            return company;
        }

        return null;
    }

    public Company? GetCompanyDetailsByName(string companyName, string? year)
    {
        using var reader = _iPeerAMidDataAccess.GetCompanyDetailsByName(companyName, year);
        if (reader.Read())
        {
            var company = ParseCompany(reader);
            // Log.Debug(JsonConvert.SerializeObject(company));
            return company;
        }

        return null;
    }

    public Company? GetCompanyDetailsByTicker(string ticker, string? year)
    {
        using var reader = _iPeerAMidDataAccess.GetCompanyDetailsByTicker(ticker, year);
        if (reader.Read())
        {
            var company = ParseCompany(reader);
            // Log.Debug(JsonConvert.SerializeObject(company));
            return company;
        }

        return null;
    }


    public List<Company> GetAdditionalPeerCompanyList(string industryId, string uid, string companyName,
        decimal revenueStart, decimal revenueTo, int year,
        string regions,
        int iSortCol0, string sSortDir0,
        int optionId, string subIndustryId,
        string? ticker = null)
    {
        Log.Info("optionId = " + optionId);
        // Log.Debug("Entered GetAdditionalPeerCompanyList");
        // Log.Debug($"industryId {industryId}");
        // Log.Debug($"uid {uid}");
        // Log.Debug($"companyName {companyName}");
        // Log.Debug($"revenueStart {revenueStart}");
        // Log.Debug($"revenueTo {revenueTo}");
        // Log.Debug($"year {year}");
        // Log.Debug($"regions {regions}");
        // Log.Debug($"pageNumber {pageNumber}");
        // Log.Debug($"pageSize {pageSize}");
        // Log.Debug($"iSortCol0 {iSortCol0}");
        // Log.Debug($"sSortDir0 {sSortDir0}");
        // Log.Debug($"userid {userid}");
        // Log.Debug($"isAdmin {isAdmin}");
        // Log.Debug($"optionId {optionId}");
        // Log.Debug($"subIndustryId {subIndustryId}");
        // Log.Debug($"ticker {ticker}");

        try
        {
            using var iReader = _iPeerAMidDataAccess.GetAdditionalPeerCompanyList(
                industryId,
                uid,
                companyName,
                revenueStart,
                revenueTo,
                year,
                regions,
                iSortCol0,
                sSortDir0,
                optionId,
                subIndustryId,
                ticker);
            if (iReader != null)
            {
                var requireWorkingCapitalData = SessionData.Instance.SelectedService == PeerAMidService.WcdFull || SessionData.Instance.SelectedService == PeerAMidService.WcdShort;
                var list = ParseAdditionalPeerCompanies(iReader, requireWorkingCapitalData);
                Log.Debug("Leaving GetAdditionalPeerCompanyList " + list.Count);
                return list;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }

        return new List<Company>();
    }

    public List<Company> GetAutoSuggestCompanyList(string companyId, int optionId,
        string year = "0", string? regions = null)
    {
        Log.Info("optionId = " + optionId);
        using var iReader = _iPeerAMidDataAccess.GetAutoSuggestCompanyList(companyId, optionId, year, regions ?? "");
        var requireWorkingCapitalData = SessionData.Instance.SelectedService == PeerAMidService.WcdFull || SessionData.Instance.SelectedService == PeerAMidService.WcdShort;
        List<Company> list = ParseAutoSuggestCompanies(iReader, requireWorkingCapitalData);
        Log.Debug("GetAutoSuggestCompanyList " + list.Count);

        return list;
    }

    public List<Company> GetSuggestedPeerCompanyList(
        int uid, decimal revenueFrom, decimal revenueTo, int year,
        string? regions,
        string? industryFilter, string? subIndustryFilter,
        string? nameFilter, string? tickerFilter,
        int maxCompanies)
    {
        // Log.Debug("Entered GetAdditionalPeerCompanyList");
        // Log.Debug($"industryId {industryId}");
        // Log.Debug($"uid {uid}");
        // Log.Debug($"companyName {companyName}");
        // Log.Debug($"revenueStart {revenueStart}");
        // Log.Debug($"revenueTo {revenueTo}");
        // Log.Debug($"year {year}");
        // Log.Debug($"regions {regions}");
        // Log.Debug($"pageNumber {pageNumber}");
        // Log.Debug($"pageSize {pageSize}");
        // Log.Debug($"iSortCol0 {iSortCol0}");
        // Log.Debug($"sSortDir0 {sSortDir0}");
        // Log.Debug($"userid {userid}");
        // Log.Debug($"isAdmin {isAdmin}");
        // Log.Debug($"optionId {optionId}");
        // Log.Debug($"subIndustryId {subIndustryId}");
        // Log.Debug($"ticker {ticker}");

        try
        {
            using var iReader = _iPeerAMidDataAccess.GetSuggestedPeerCompanyList(
                uid, revenueFrom, revenueTo, year,
                regions,
                industryFilter, subIndustryFilter,
                nameFilter, tickerFilter,
                maxCompanies);

            var requireWorkingCapitalData = SessionData.Instance.SelectedService == PeerAMidService.WcdFull || SessionData.Instance.SelectedService == PeerAMidService.WcdShort;
            var list = ParseAdditionalPeerCompanies(iReader, requireWorkingCapitalData);
            Log.Debug("Leaving GetSuggestedPeerCompanyList " + list.Count);
            return list;

        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }

        return new List<Company>();
    }



    public List<Company> GetFinalPeersCompanyList(string possiblePeer, string selfPeer, string year, int optionId, string? regions)
    {
        Log.Info("optionId = " + optionId);
        using var iReader = _iPeerAMidDataAccess.GetFinalPeersCompanyList(
            possiblePeer,
            selfPeer,
            year,
            optionId,
            regions ?? "");
        return ParseFinalPeersCompanies(iReader);
    }

    public List<Company> GetPreviouslySelectedPeers(string companyId)
    {
        using var reader = _iPeerAMidDataAccess.GetPreviouslySelectedPeers(companyId);
        return ParseFinalPeersCompanies(reader);
    }

    public List<Company> GetCompanyRequiredData(string companyId)
    {
        var list = new List<Company>();
        using var ireader = _iPeerAMidDataAccess.GetCompanyRequiredData(companyId);
        while (ireader.Read())
        {
            try
            {
                var model = new Company();
                model.DataEntryCurrency = DBDataHelper.GetString(ireader, "Currency");
                model.CompanyName = DBDataHelper.GetString(ireader, "ComPanyName");
                model.Revenue = DBDataHelper.GetDecimal(ireader, "Revenue");
                model.FinancialYear = DBDataHelper.GetInt(ireader, "FY");
                model.Ebitda = DBDataHelper.GetDecimal(ireader, "EBITDA");
                model.GrossMargin = DBDataHelper.GetDecimal(ireader, "GrossMargin");
                model.Expense = DBDataHelper.GetDecimal(ireader, "SGA");
                model.DataEntryUnitOfMeasure = DBDataHelper.GetString(ireader, "CostUnit");
                model.Ticker = DBDataHelper.GetString(ireader, "Ticker");
                model.CompanyType = DBDataHelper.GetString(ireader, "CompanyType");
                model.IndustryId = DBDataHelper.GetInt(ireader, "IndustryId");
                model.SubIndustryId = DBDataHelper.GetInt(ireader, "SubIndustryId");
                model.TotalEmployees = DBDataHelper.GetDecimal(ireader, "TotalEmployee");
                model.Country = DBDataHelper.GetString(ireader, "Country");
                model.ShortName = DBDataHelper.GetString(ireader, "ShortName");
                model.CompanyNameMixedCase = DBDataHelper.GetString(ireader, "CompanyNameMixedCase");
                model.ShortNameMixedCase = DBDataHelper.GetString(ireader, "ShortNameMixedCase");
                list.Add(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            try
            {
                var model = new Company();
                model.DataEntryCurrency = DBDataHelper.GetString(ireader, "Currency").Trim();
                model.CompanyName = DBDataHelper.GetString(ireader, "ComPanyName").Trim();
                model.Revenue = DBDataHelper.GetDecimal(ireader, "Revenue");
                model.FinancialYear = DBDataHelper.GetInt(ireader, "FY");
                model.Ebitda = DBDataHelper.GetDecimal(ireader, "EBITDA");
                model.GrossMargin = DBDataHelper.GetDecimal(ireader, "GrossMargin");
                model.Expense = DBDataHelper.GetDecimal(ireader, "SGA");
                model.DataEntryUnitOfMeasure = DBDataHelper.GetString(ireader, "CostUnit").Trim();
                model.Ticker = DBDataHelper.GetString(ireader, "Ticker").Trim();
                model.CompanyType = DBDataHelper.GetString(ireader, "CompanyType").Trim();
                model.IndustryId = DBDataHelper.GetInt(ireader, "IndustryId");
                model.SubIndustryId = DBDataHelper.GetInt(ireader, "SubIndustryId");
                model.TotalEmployees = DBDataHelper.GetDecimal(ireader, "TotalEmployee");
                model.Country = DBDataHelper.GetString(ireader, "Country").Trim();
                model.ShortName = DBDataHelper.GetString(ireader, "ShortName").Trim();
                model.CompanyNameMixedCase = DBDataHelper.GetString(ireader, "CompanyNameMixedCase").Trim();
                model.ShortNameMixedCase = DBDataHelper.GetString(ireader, "ShortNameMixedCase").Trim();
                list.Add(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        return list;
    }

    public List<Company> GetCompanyRequiredDataHistory(string companyId)
    {
        var list = new List<Company>();
        using var ireader = _iPeerAMidDataAccess.GetCompanyRequiredDataHistory(companyId);
        while (ireader.Read())
        {
            try
            {
                var model = new Company();
                ParseCompany(ireader, model);
                list.Add(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        return list;
    }

    public ActualDataCollectionModel GetClientFunctionalData(string companyId, string year)
    {
        var model = new ActualDataCollectionModel();
        using var reader = _iPeerAMidDataAccess.GetClientFunctionalData(companyId, year);
        while (reader.Read())
        {
            try
            {
                ParseCompany(reader, model); // works for ActualDataCollectionMode, too
                //model.YSCID = DBDataHelper.GetInt(reader, "YSCID");
                model.SGACostFinance = DBDataHelper.GetNullableDouble(reader, "FNCOSTMN");
                model.SGACostHumanResources = DBDataHelper.GetNullableDouble(reader, "HRCOSTMN");
                model.SGACostIT = DBDataHelper.GetNullableDouble(reader, "ITCOSTMN");
                model.SGACostProcurement = DBDataHelper.GetNullableDouble(reader, "PRCOSTMN");
                model.SGACostCorporateSupportServices = DBDataHelper.GetNullableDouble(reader, "CORPCOSTMN");
                model.SGACostSales = DBDataHelper.GetNullableDouble(reader, "SCOSTMN");
                model.SGACostCustomerServices = DBDataHelper.GetNullableDouble(reader, "CSCOSTMN");
                model.SGACostMarketing = DBDataHelper.GetNullableDouble(reader, "MKCOSTMN");
                //model.EBITDA = DBDataHelper.GetNullableDouble(reader,                          "EBITDAMN");
                model.FTEFinance = DBDataHelper.GetNullableDouble(reader, "FNFTEA");
                model.FTEHumanResources = DBDataHelper.GetNullableDouble(reader, "HRFTEA");
                model.FTEIT = DBDataHelper.GetNullableDouble(reader, "ITFTEA");
                model.FTEProcurement = DBDataHelper.GetNullableDouble(reader, "PRFTEA");
                model.FTECorporateSupportServices = DBDataHelper.GetNullableDouble(reader, "CORPFTEA");
                model.FTESales = DBDataHelper.GetNullableDouble(reader, "SFTEA");
                model.FTECustomerServices = DBDataHelper.GetNullableDouble(reader, "CSFTEA");
                model.FTEHumanResources = DBDataHelper.GetNullableDouble(reader, "HRFTEA");
                model.FTEMarketing = DBDataHelper.GetNullableDouble(reader, "MKFTEA");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        return model;
    }

    public int SaveRunAnalysis(string filename, string details)
    {
        return _iPeerAMidDataAccess.SaveRunAnalysis(filename, details);
    }

    public DataTable GetMinMaxRevenueOfCurrentYear()
    {
        return _iPeerAMidDataAccess.GetMinMaxRevenueOfCurrentYear();
    }

    public int GetCurrentFinancialYear()
    {
        var year = 0;
        try
        {
            if (_iPeerAMidDataAccess == null)
                throw new NullReferenceException("_iPeerAMidDataAccess is null");
            var ireader = _iPeerAMidDataAccess.GetCurrentFinancialYear();
            if (ireader == null)
                throw new NullReferenceException("ireader ia null");

            using (ireader)
            {
                if (!ireader.Read())
                    throw new Exception("Could not read");
                year = DBDataHelper.GetInt(ireader, "CurrentFiscalYear");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }

        return year;
    }

    public ApplicationConfig GetCurrentFinancialData()
    {
        var appSetting = new ApplicationConfig();
        using var ireader = _iPeerAMidDataAccess.GetCurrentFinancialYear();
        while (ireader.Read())
        {
            try
            {
                appSetting.SettingId = DBDataHelper.GetInt(ireader, "SettingId");
                appSetting.CurrentFiscalYear = DBDataHelper.GetInt(ireader, "CurrentFiscalYear");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        return appSetting;
    }

    public int UpdateCurrentFinancialYear(ApplicationConfig appConfig)
    {
        try
        {
            return _iPeerAMidDataAccess.UpdateCurrentFinancialYear(
                appConfig.SettingId,
                appConfig.CurrentFiscalYear);
        }
        catch (Exception ex)
        {
            Log.Error(ex);

            return 0;
        }
    }

    private static List<Company> ProcessBenchmarkCompanyList(IDataReader reader, bool requireWorkingCapitalData)
    {
        Log.Debug("Entering ProcessBenchmarkCompanyList(_," + requireWorkingCapitalData + ")");
        var list = ParseCompanies(reader);
        Log.Debug("Parsed");
        try
        {
            var nastyIndices = new List<int>();
            var duplicatesChecker = new SortedList<string, int>(); // CompanyName -> index

            for (var index = 0; index < list.Count; ++index)
            {
                var company = list[index];
                if (requireWorkingCapitalData && !company.HasWorkingCapitalData)
                {
                    nastyIndices.Add(index);
                }
                else if (duplicatesChecker.TryGetValue(company.Name!, out var i))
                {
                    // Log.Debug("Is duplicate of " + i);
                    if (list[i].DataYear < company.DataYear)
                        // Log.Debug("Is younger duplicate of " + i);
                        nastyIndices.Add(i);
                    // Log.Debug("Is older duplicate of " + i);
                }
                else
                {
                    duplicatesChecker.Add(company.Name!, index);
                }
            }

            nastyIndices.Sort();
            nastyIndices.Reverse();
            foreach (var index in nastyIndices)
                list.RemoveAt(index);
        }
        catch (Exception e)
        {
            Log.Error(e);
        }

        Log.Debug("Exiting ProcessBenchmarkCompanyList with " + list.Count + "entries");
        return list;
    }

    private static List<Company> ParseAutoSuggestCompanies(IDataReader reader, bool requireWorkingCapitalData)
    {
        var list = ParseCompanies(reader, c => { c.IsSuggested = true; });
        if (requireWorkingCapitalData)
        {
            var index = 0;
            while (index < list.Count)
            {
                if (list[index].HasWorkingCapitalData)
                    ++index;
                else
                    list.RemoveAt(index);
            }
        }

        return list;
    }

    private List<Company> ParseAdditionalPeerCompanies(IDataReader iReader, bool requireWorkingCapitalData)
    {
        var max = 500;
        //Log.Debug("Entered ParseAdditionalPeerCompanies");

        var list = new List<Company>();
        var duplicatesChecker = new Dictionary<string, int>(); // Company Name -> Index
        while (iReader.Read())
        {
            try
            {
                Company model;
                var companyName = DBDataHelper.GetString(iReader, "ComPanyName");
                var dataYear = DBDataHelper.GetInt(iReader, "FY");

                //Log.Debug($"Read {companyName} {dataYear}");
                if (duplicatesChecker.TryGetValue(companyName, out var index))
                {
                    if (list[index].DataYear < dataYear)
                        model = list[index];
                    else
                        //Log.Debug($"IsDuplicate! {companyName} {dataYear}");
                        continue;
                }
                else
                {
                    model = new Company
                    {
                        Name = companyName
                    };
                    duplicatesChecker.Add(companyName, list.Count);
                    list.Add(model);
                    //Log.Debug($"Added to list! {companyName} {dataYear}");
                }

                ParseCompany(iReader, model);
                if (requireWorkingCapitalData && !model.HasWorkingCapitalData)
                {
                    list.RemoveAt(list.Count - 1);
                }
                else
                {
                    if (list.Count >= max)
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        //Log.Debug("Leaving ParseAdditionalPeerCompanies");
        return list;
    }

    public List<Company> ParseFinalPeersCompanies(IDataReader reader)
    {
        return ParseCompanies(reader);
    }

    internal static List<Company> ParseCompanies(IDataReader reader, Action<Company>? postProcessor = null)
    {
        var list = new List<Company>();
        while (reader.Read())
        {
            var company = ParseCompany(reader, postProcessor);
            if (company != null)
                list.Add(company);
        }

        return list;
    }

    internal static Company? ParseCompany(IDataReader reader, Company model)
    {
        return ParseCompany(reader, null, model);
    }

    internal static Company? ParseCompany(IDataReader reader, Action<Company>? postProcessor = null, Company? model = null)
    {
        model ??= new Company();
        var name = "none";
        var f = -1;

        try
        {
            for (f = 0; f < reader.FieldCount; ++f)
            {
                name = reader.GetName(f);
                switch (name.ToUpper())
                {
                    case "COMPANYNAME":
                        model.Name = reader.ToString(f) ?? "";
                        break;

                    case "COMPANYNAMEMIXEDCASE":
                        model.CompanyNameMixedCase = reader.ToString(f) ?? "";
                        break;

                    case "COMPANYTYPE":
                        model.CompanyType = reader.ToString(f) ?? "";
                        break;


                    case "COSTUNIT":
                        //Debug.WriteLine("ParseCompany:" + name);
                        model.DataEntryUnitOfMeasure = reader.ToString(f)?.Trim() ?? "";
                        break;

                    case "COUNTRY":
                        model.Country = reader.ToString(f) ?? "";
                        break;

                    case "CURRENCY":
                    case "REPCURR":
                        model.DataEntryCurrency = reader.ToString(f)?.Trim() ?? "";
                        break;

                    case "DATAYEAR":
                    case "DATAYR":
                        model.DataYear = reader.ToInt(f) ?? 0;
                        if (model.FinancialYear == 0)
                            model.FinancialYear = model.DataYear;
                        break;

                    case "EBITDA":
                    case "EBITDAMN":
                        model.Ebitda = reader.ToDecimal(f) ?? 0;
                        break;

                    case "EM1":
                    case "EBITDAMARGIN":
                        model.EbitdaMargin = reader.ToDecimal(f) ?? 0;
                        break;

                    case "EXCHANGERATE":
                    case "EXCHANGERATENUMBER":
                    case "EXRATE":
                        //Debug.WriteLine("ParseCompany:" + name);
                        model.DataEntryExchangeRate = reader.ToDouble(f) ?? 1;
                        break;

                    case "FINANCIALYEAR":
                    case "FY":
                        model.FinancialYear = reader.ToInt(f) ?? 0;
                        if (model.DataYear == 0)
                            model.DataYear = model.FinancialYear;
                        break;

                    case "GROSSMARGIN":
                    case "GROSSMN":
                        model.GrossMargin = reader.ToDecimal(f) ?? 0;
                        break;

                    case "HASWORKINGCAPITALDATA":
                        model.HasWorkingCapitalData = reader.ToInt(f) != 0;
                        break;

                    case "INDUSTRY":
                        model.Industry = reader.ToString(f) ?? "";
                        break;

                    case "INDUSTRYID":
                        model.IndustryId = reader.ToInt(f) ?? 0;
                        break;

                    case "ISACTUAL":
                        model.IsActualData = reader.ToInt(f) != 0;
                        break;

                    case "ISSUGGESTED":
                        model.IsSuggested = reader.ToBoolean(f) ?? false;
                        break;

                    case "ISTARGET":
                        // Do nothing
                        break;

                    case "NOOFRECORDS":
                        model.TotalRecords = reader.ToInt(f) ?? 0;
                        break;

                    case "REVENUE":
                    case "REVMNA":
                    case "REVNMA": // mistake?
                        model.Revenue = reader.ToDecimal(f) ?? 0;
                        break;

                    case "SGAMN":
                    case "SGA1":
                        model.Expense = reader.ToDecimal(f) ?? 0;
                        break;

                    case "SHORTNAME":
                        model.ShortName = reader.ToString(f) ?? "";
                        break;

                    case "SHORTNAMEMIXEDCASE":
                        model.ShortNameMixedCase = reader.ToString(f) ?? "";
                        // Log.Debug("Read SNMC as " + model.ShortNameMixedCase);
                        break;

                    case "SICDESCRIPTION":
                    case "SUBINDUSTRY":
                        model.SubIndustry = reader.ToString(f) ?? "";
                        break;

                    case "SIC_CODE":
                    case "SUBINDUSTRYID":
                        model.SubIndustryId = reader.ToInt(f) ?? 0;
                        break;

                    case "TICKER":
                        model.Ticker = reader.ToString(f) ?? "";
                        break;

                    case "TOTALEMPLOYEE":
                    case "TOTEE":
                    case "TOTALNUMOFEMPLOYEE":
                        model.TotalEmployees = reader.ToDecimal(f) ?? 0;
                        break;

                    case "UID":
                        model.Id = reader.ToString(f) ?? "";
                        break;

                    case "YSCID":
                    case "YSID":
                        model.Yscid = reader.ToString(f) ?? "";
                        break;

                    case "FNCOSTMN":
                        if (model is ActualDataCollectionModel && !reader.IsDBNull(f))
                            ((ActualDataCollectionModel)model).SGACostFinance = double.Parse(reader.ToString(f));
                        break;

                    case "HRCOSTMN":
                        if (model is ActualDataCollectionModel && !reader.IsDBNull(f))
                            ((ActualDataCollectionModel)model).SGACostHumanResources = double.Parse(reader.ToString(f));
                        break;

                    case "ITCOSTMN":
                        if (model is ActualDataCollectionModel && !reader.IsDBNull(f))
                            ((ActualDataCollectionModel)model).SGACostIT = double.Parse(reader.ToString(f));
                        break;

                    case "PRCOSTMN":
                        if (model is ActualDataCollectionModel && !reader.IsDBNull(f))
                            ((ActualDataCollectionModel)model).SGACostProcurement = double.Parse(reader.ToString(f));
                        break;

                    case "CORPCOSTMN":
                        if (model is ActualDataCollectionModel && !reader.IsDBNull(f))
                            ((ActualDataCollectionModel)model).SGACostCorporateSupportServices = double.Parse(reader.ToString(f));
                        break;

                    case "SCOSTMN":
                        if (model is ActualDataCollectionModel && !reader.IsDBNull(f))
                            ((ActualDataCollectionModel)model).SGACostSales = double.Parse(reader.ToString(f));
                        break;

                    case "CSCOSTMN":
                        if (model is ActualDataCollectionModel && !reader.IsDBNull(f))
                            ((ActualDataCollectionModel)model).SGACostCustomerServices = double.Parse(reader.ToString(f));
                        break;

                    case "MKCOSTMN":
                        if (model is ActualDataCollectionModel && !reader.IsDBNull(f))
                            ((ActualDataCollectionModel)model).SGACostMarketing = double.Parse(reader.ToString(f));
                        break;

                    case "FNFTEA":
                        if (model is ActualDataCollectionModel && !reader.IsDBNull(f))
                            ((ActualDataCollectionModel)model).FTEFinance = double.Parse(reader.ToString(f));
                        break;

                    case "HRFTEA":
                        if (model is ActualDataCollectionModel && !reader.IsDBNull(f))
                            ((ActualDataCollectionModel)model).FTEHumanResources = double.Parse(reader.ToString(f));
                        break;

                    case "ITFTEA":
                        if (model is ActualDataCollectionModel && !reader.IsDBNull(f))
                            ((ActualDataCollectionModel)model).FTEIT = double.Parse(reader.ToString(f));
                        break;

                    case "PRFTEA":
                        if (model is ActualDataCollectionModel && !reader.IsDBNull(f))
                            ((ActualDataCollectionModel)model).FTEProcurement = double.Parse(reader.ToString(f));
                        break;

                    case "CORPFTEA":
                        if (model is ActualDataCollectionModel && !reader.IsDBNull(f))
                            ((ActualDataCollectionModel)model).FTECorporateSupportServices = double.Parse(reader.ToString(f));
                        break;

                    case "SFTEA":
                        if (model is ActualDataCollectionModel && !reader.IsDBNull(f))
                            ((ActualDataCollectionModel)model).FTESales = double.Parse(reader.ToString(f));
                        break;

                    case "CSFTEA":
                        if (model is ActualDataCollectionModel && !reader.IsDBNull(f))
                            ((ActualDataCollectionModel)model).FTECustomerServices = double.Parse(reader.ToString(f));
                        break;

                    case "MKFTEA":
                        if (model is ActualDataCollectionModel && !reader.IsDBNull(f))
                            ((ActualDataCollectionModel)model).FTEMarketing = double.Parse(reader.ToString(f));
                        break;

                    case "DATAENTRYCURRENCY":
                        //Debug.WriteLine("ParseCompany:" + name);
                        model.DataEntryCurrency = reader.ToString(f) ?? "";
                        break;

                    case "DATAENTRYEXCHANGERATE":
                        //Debug.WriteLine("ParseCompany:" + name);
                        model.DataEntryExchangeRate = reader.ToDouble(f) ?? 1;
                        break;

                    case "DATAENTRYUNITOFMEASURE":
                        //Debug.WriteLine("ParseCompany:" + name + ":" + reader.GetValue(f).ToString());
                        model.DataEntryUnitOfMeasure = reader.ToString(f)?.Trim() ?? "";
                        break;

                    case "SGA":
                        model.SGA = reader.ToDecimal(f) ?? 0;
                        break;

                    case "ISACTUALDATA":
                        model.IsActualData = (reader.ToInt(f) ?? 0) != 0;
                        break;

                    default:
                        Log.Error("Unexpected field: " + name);
                        //Debugger.Break();
                        break;
                }
            }

            name = "No field name";

            postProcessor?.Invoke(model);

            if (string.IsNullOrEmpty(model.ShortNameMixedCase))
                throw new Exception("No ShortNameMixedCase");
            return model;
        }
        catch (Exception ex)
        {
            Log.Error("Exception while handling " + name + " " + reader.GetFieldType(f));
            Log.Error(ex);
            return null;
        }
    }
}
