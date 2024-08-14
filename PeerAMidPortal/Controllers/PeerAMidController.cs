using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using PeerAMid.Business;
using PeerAMid.Core;
using PeerAMid.Data;
using PeerAMid.Support;
using PeerAMid.Utility;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

#nullable enable

namespace YardStickPortal;

/// <summary>
///     Maximum function of application put under below class(Create new company, Edit company, Search data, Save data,
///     GetTargetCompanyDetails, BenchmarkCompanySearchResults..etc).
///     Session related all changes(Data insert in session, Clear data from session) only handle in PeerAMidController
///     class.
///     PPT, Excel creation code written PeerAMidController class.
///     When any request come from UI end then first request handle by below class and then we are calling API which is
///     internally calling data layer for fetch and save data in database.
/// </summary>
//[Authorize]
public partial class PeerAMidController : Controller
{
    private readonly IHomeCore _homeCore;
    private readonly IPeerAMidCore _iPeerAMid;
    public readonly IActualDataCollectionCore ActualDataCollectionCore;
    private static readonly object Obj = new();
    private readonly AccountController _accountController;
    public static bool LogApiCalls { get; set; } = true;


    private string ActionName => ControllerContext.RouteData.Values["action"].ToString();
    private string ControllerName => ControllerContext.RouteData.Values["controller"].ToString();
    private string PagePath => ControllerName + "/" + ActionName;
    private SessionData SessionData => SessionData.Instance;


    public PeerAMidController(IHomeCore homeCore, IPeerAMidCore iPeerAMid,
        IActualDataCollectionCore actualDataCollectionCore,
        AccountController accountController)
    {
        _homeCore = homeCore;
        _iPeerAMid = iPeerAMid;
        ActualDataCollectionCore = actualDataCollectionCore;
        _accountController = accountController;
    }


    [CustomAuthorization]
    public ActionResult Index()
    {
        return View();
    }

    /// <summary>
    ///     Company Search  - Initiates the "SGA Diagnostic" and "SGA High Level Cost" processes.
    /// </summary>
    /// <param name="serviceSelection"></param>
    /// <returns></returns>
    /// <summary>
    ///     Set session data in application and redirect in search page. Proc_GetCurrentFinancialYear
    /// </summary>
    /// <returns></returns>
    [CustomAuthorization]
    public ActionResult Search(PeerAMidService serviceSelection = PeerAMidService.None)
    {
        // Log.Debug("in Search() with " + ServiceSelection);

        // Check if a PeerAMid service was provided.
        if (serviceSelection == PeerAMidService.None)
        {
            serviceSelection = SessionData.SelectedService;
            if (serviceSelection == PeerAMidService.None)
                SessionData.SelectedService = serviceSelection = PeerAMidService.SgaShort;
        }
        else
        {
            SessionData.SelectedService = serviceSelection;
        }

        // Don't allow the user to access the SG&A Diagnostic if they're not authorized or an Admin (PEER-10)
        if (serviceSelection == PeerAMidService.SgaFull &&
            SessionData.User is { AllowFullSGA: false, IsAdmin: false })
        {
            // Redirect user to default "Search" action
            Log.Error($"Unauthorized Service Selection value of [{serviceSelection}]. Redirecting to default 'Search' action.\r\n[{PagePath}]");
            return RedirectToAction("Search", new { ServiceSelection = PeerAMidService.SgaShort });
        }

        // Create new session wrapper
        SessionData.CurrentFinancialYear = _iPeerAMid.GetCurrentFinancialYear();
        TempData.Remove("BackButtonPeerSel");
        SessionData.PeerSelOption = 0;
        return View();
    }

    /// <summary>
    ///     Load Industry data in dropdown, add session value and return add new company creation page.
    /// </summary>
    /// <returns></returns>
    [CustomAuthorization]
    public ActionResult AddNewCompany()
    {
        // Get current service selection
        var serviceSelection = SessionData.SelectedService;

        // If service selection is invalid, redirect user to main application page (PEER-14)
        if (serviceSelection == PeerAMidService.None) // replace this logic with policy check
        {
            Log.Error(
                $"Invalid or null Service Selection value. Redirecting to main application page.\r\n[{PagePath}]");
            return RedirectToAction("Index");
        }

        // Validate the authorization of the service selection  (PEER-14)
        if (serviceSelection == PeerAMidService.SgaFull &&
            SessionData.User is { AllowFullSGA: false, IsAdmin: false })
        {
            // Unauthorized service selection value - redirect user to main application page
            Log.Error(
                $"Unauthorized Service Selection value of [{serviceSelection}]. Redirecting to main application page.\r\n[{PagePath}]");
            return RedirectToAction("Index");
        }

        SessionData.PeerSelOption = 3;
        TempData["BackButtonPeerSel"] = 3;
        //dynamic mymodel = new ExpandoObject();
        //mymodel.IndustryList = _homeCore.GetIndustryList("")!;
        return View("NewCompany");
    }

    /// <summary>
    ///     After click on Next button Save latest company data and redirect in RunAnalysis.cshtml
    ///     page.
    /// </summary>
    /// <returns></returns>
    [CustomAuthorization]
    [HttpPost]
    [CalledFromExternalCode]
    public ActionResult SaveActualDataCollectionEditNew()
    {
        var model = ParseBodyIntoModel();
        //  SessionData.SubIndustry=model.SubIndustryId;
        SessionData.BenchmarkCompany.FinancialYear = model.YearId;
        model.CompanyUID = Convert.ToInt32(SessionData.BenchmarkCompany.Id);
        model.UserId = SessionData.User.Id!;

        var result = ActualDataCollectionCore.SaveActualDataCollectionEdit(model, model.CompanyUID);
        //Session["UID"] = result;
        model.Result = result;
        return Json(model, JsonRequestBehavior.AllowGet);
    }

    /// <summary>
    ///     Save new company’s actual data in database.
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [CustomAuthorization]
    [CalledFromExternalCode]
    public JsonResult SaveActualDataCollectionNew()
    {
        var model = ParseBodyIntoModel();
        var newUid = ActualDataCollectionCore.SaveActualDataCollection(model);
        if (newUid == -1)
        {
            var badCompany = new Company
            {
                Id = newUid.ToString()
            };
            return new JsonResult { Data = badCompany, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        var company = _iPeerAMid.GetCompanyDetails(newUid, model.YearId)!;
        SessionData.BenchmarkCompany.Id = company.Id;
        SessionData.BenchmarkCompany.DataYear = model.DataYear;
        SessionData.BenchmarkCompany.SubIndustryId = model.SubIndustryId;
        SessionData.BenchmarkCompany.FinancialYear = model.YearId;
        return new JsonResult { Data = company, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
    }

    private ActualDataCollectionModel ParseBodyIntoModel()
    {
        var model = new ActualDataCollectionModel();
        var p = Request.Params;

        foreach (string key in p)
        {
            var k = key[0] == '\"' ? key.Substring(1) : key;
            var v = p[key];
            if (v.EndsWith("\""))
                v = v.Substring(0, v.Length - 1);
            int i;
            decimal d;
            double f;
            switch (k.ToUpper())
            {
                case "COMPANYID":
                    model.CompanyId = v;
                    break;
                case "UNITOFMEASUREMENT":
                case "UNITOFMEASURE":
                    model.DataEntryUnitOfMeasure = v;
                    break;
                case "CURRENCYID":
                    model.DataEntryCurrency = v.ToUpper();
                    break;
                case "COMPANYTYPE":
                    model.CompanyType = v;
                    break;
                case "COMPANYNAME":
                    model.CompanyName = v;
                    break;
                case "USERID":
                    model.UserId = v;
                    break;
                case "TICKER":
                    model.Ticker = v;
                    break;
                case "INDUSTRYID":
                    model.IndustryId = int.Parse(v);
                    break;
                case "SUBINDUSTRYID":
                    model.SubIndustryId = int.Parse(v);
                    break;
                case "YEARID":
                    if (int.TryParse(v, out i))
                        model.YearId = i;
                    break;
                case "TOTALNUMOFEMPLOYEE":
                    if (int.TryParse(v.Replace(",",""), out i))
                        model.TotalNumOfEmployee = i;
                    break;
                case "ISOPTIONALDATA":
                    if (int.TryParse(v, out i))
                        model.IsOptionalData = i;
                    break;
                case "REVENUE":
                    if (decimal.TryParse(v, out d))
                        model.Revenue = d;
                    break;
                case "EBITDA":
                    if (decimal.TryParse(v, out d))
                        model.Ebitda = d;
                    break;
                case "SGA":
                    if (decimal.TryParse(v, out d))
                        model.SGA = d;
                    break;
                case "GROSSMARGIN":
                    if (decimal.TryParse(v, out d))
                        model.GrossMargin = d;
                    break;
                case "COUNTRY":
                    model.Country = v;
                    break;
                case "SGACOSTFINANCE":
                    if (double.TryParse(v, out f))
                        model.SGACostFinance = f;
                    break;

                case "SGACOSTHUMANRESOURCES":
                    if (double.TryParse(v, out f))
                        model.SGACostHumanResources = f;
                    break;
                case "SGACOSTIT":
                    if (double.TryParse(v, out f))
                        model.SGACostIT = f;
                    break;
                case "SGACOSTPROCUREMENT":
                    if (double.TryParse(v, out f))
                        model.SGACostProcurement = f;
                    break;
                case "SGACOSTCORPORATESUPPORTSERVICES":
                    if (double.TryParse(v, out f))
                        model.SGACostCorporateSupportServices = f;
                    break;
                case "SGACOSTCUSTOMERSERVICES":
                    if (double.TryParse(v, out f))
                        model.SGACostCustomerServices = f;
                    break;
                case "SGACOSTSALES":
                    if (double.TryParse(v, out f))
                        model.SGACostSales = f;
                    break;

                case "SGACOSTMARKETING":
                    if (double.TryParse(v, out f))
                        model.SGACostMarketing = f;
                    break;
                case "FTEFINANCE":
                    if (double.TryParse(v, out f))
                        model.FTEFinance = f;
                    break;
                case "FTEHUMANRESOURCES":
                    if (double.TryParse(v, out f))
                        model.FTEHumanResources = f;
                    break;
                case "FTEIT":
                    if (double.TryParse(v, out f))
                        model.FTEIT = f;
                    break;
                case "FTEPROCUREMENT":
                    if (double.TryParse(v, out f))
                        model.FTEProcurement = f;
                    break;
                case "FTECORPORATESUPPORTSERVICES":
                    if (double.TryParse(v, out f))
                        model.FTECorporateSupportServices = f;
                    break;
                case "FTECUSTOMERSERVICES":
                    if (double.TryParse(v, out f))
                        model.FTECustomerServices = f;
                    break;
                case "FTESALES":
                    if (double.TryParse(v, out f))
                        model.FTESales = f;
                    break;
                case "FTEMARKETING":
                    if (double.TryParse(v, out f))
                        model.FTEMarketing = f;
                    break;
                case "EXCHANGERATE":
                    if (v.IndexOf(',') > 0)
                        v = v.Substring(0, v.IndexOf(','));
                    if (double.TryParse(v, out f))
                        model.DataEntryExchangeRate = f;
                    else
                        model.DataEntryExchangeRate = 1;
                    break;
            }
        }

        model.CurrentCurrencySettings = model.DataEntryCurrencySettings; // the body always comes from data entry
        return model;
    }


    [HttpGet]
    [CustomAuthorization]
    public ActualDataCollectionModel? GetFilledDataForYear(string companyUid, string yearId)
    {
        var model = ActualDataCollectionCore.GetFilledDataForYear(companyUid, yearId);
        if (model == null)
            return null;
        return CostFieldsCalculation.ConvertCurrencyFields(model, model.DataEntryCurrencySettings)!;
    }

    /// <summary>
    ///     Load previous financial year and current financial year data and bind all controls in EditNewCompany.cshtml page.
    ///     In EditNewCompany page there is option to click on back button after click the back button below function will call
    ///     and redirect to EditNewCompany.cshtml page. Proc_GetClientActualData  Proc_GetCompanyDetail
    /// </summary>
    /// <returns></returns>
    [CustomAuthorization]
    public ActionResult EditNewCompany()
    {
#if false
        SessionData.PeerSelOption = 3;
        TempData["BackButtonPeerSel"] = 3;
        dynamic mymodel = new ExpandoObject();
        //mymodel.IndustryList = _homeCore.GetIndustryList("");

        ActualDataCollectionModel? model = null;

        var targetId = SessionData.BenchmarkCompany.Id;
        Company? company = null;
        var year = SessionData.BenchmarkCompany.FinancialYear;

        //var userId = SessionData.User.Id;
        //var optionId = SessionData.OptionId;

        NotParallel.Invoke(
            () => model = ActualDataCollectionCore.GetFilledDataForYear(targetId!, year.ToString()),
            () => company = _iPeerAMid.GetCompanyDetails(
                targetId!,
                year.ToString()) //.Where(S => S.FinancialYear == Convert.ToInt32(year)).FirstOrDefault()
        );

        //model.DataEntryUnitOfMeasure = list.DataEntryUnitOfMeasure;
        mymodel.SubIndustryList =
            _homeCore.GetSubIndustryList(
                company!.IndustryId,
                Convert.ToString(company!.IndustryId))! /*.Where(s => s.IndustryId == list.IndustryId)*/;

        model!.YearId = Convert.ToInt32(year);
        mymodel.formData = CostFieldsCalculation.ConvertCurrencyFields(model, model.DataEntryCurrencySettings);
        mymodel.CompanyID = targetId;
        mymodel.companyData = company;
        mymodel.CompanyModel = company;
        mymodel.SubIndustryId = company.SubIndustryId;
        mymodel.SubIndustry = company.GetSubIndustry();
#endif
        return View("NewCompany");
    }

    /// <summary>
    ///     After click on search button search company data and bind in Company grid list. Proc_GetAllcompanyList
    /// </summary>
    /// <param name="cSearch"></param>
    /// <param name="tSearch"></param>
    /// <param name="regions"></param>
    /// <returns></returns>
    [CustomAuthorization]
    [HttpPost]
    [CalledFromExternalCode]
    public JsonResult BenchmarkCompanySearchResults(string cSearch, string tSearch, string regions)
    {
        //if (param.iDisplayLength != 0)
        //{
        //    pageNumber = (param.iDisplayStart / param.iDisplayLength) + 1;
        //    pageSize = param.iDisplayLength;
        //}
        SessionData.BenchmarkCompanySearchResults = _iPeerAMid.GetBenchmarkCompanyList(
            cSearch,
            tSearch,
            regions,
            "0");
        return Json(
            new
            {
                //sEcho = param.sEcho,
                //iTotalRecords = (mod.Count() > 0) ? mod.FirstOrDefault().TotalRecords : 0,
                //iTotalDisplayRecords = (mod.Count() > 0) ? mod.FirstOrDefault().TotalRecords : 0,
                aaData = SessionData.BenchmarkCompanySearchResults
            },
            JsonRequestBehavior.AllowGet);
    }

    /// <summary>
    ///     On the basis of companyId and year bind the Benchmark Company Detalis In search page.Proc_GetCompanyDetail
    /// </summary>
    /// <param name="companyId"></param>
    /// <param name="year"></param>
    /// <returns></returns>
    [CustomAuthorization]
    [CalledFromExternalCode]
    public JsonResult GetCompanyDetails(string companyId, string? year = null)
    {
        // Log.Debug("GetCompanyDetails: " + companyId + ", " + (year ?? "null"));
        var model = _iPeerAMid.GetCompanyDetails(companyId, year)!;
        if (companyId == SessionData.BenchmarkCompany.Id)
            SessionData.BenchmarkCompany.CopyFrom(model);
        return Json(model, JsonRequestBehavior.AllowGet);
    }



    /// <summary>
    ///     Get Benchmark Company data in Peer selection option page. Proc_GetCompanyDetail
    /// </summary>
    /// <returns></returns>
    [CustomAuthorization]
    public ActionResult PeerSelectionOption()
    {
        dynamic newCompanyFlag = TempData.Peek("BackButtonPeerSel") ?? 0;
        var model = _iPeerAMid.GetCompanyDetails(
            SessionData.BenchmarkCompany.Id!,
            SessionData.BenchmarkCompany.DataYear.ToString());
        model ??= _iPeerAMid.GetCompanyDetailsByName(SessionData.cSearch!, SessionData.CurrentFinancialYear.ToString());
        SessionData.BenchmarkCompany.CopyFrom(model!);
        SessionData.PeerSelOption = newCompanyFlag.ToString() == "3" ? 3 : 0;
        SessionData.IsPossiblePeerCompanies = false;
        SessionData.IsSelfPeerCompanies = false;
        return View(model);
    }

    /// <summary>
    ///    Set the selected service
    /// </summary>
    /// <returns></returns>
    [CustomAuthorization]
    public JsonResult SelectService(PeerAMidService selectedService)
    {
        SessionData.Instance.SelectedService = selectedService;
        return Json(new { }, JsonRequestBehavior.AllowGet);

    }


    /// <summary>
    ///     Load benchmark Company data in PeerSelectionAutoOrSelf.cshtml page. Proc_GetCompanyDetail
    ///     After click on Back button request redirect on PeerSelectionAutoOrSelf.cshtml page and load data via
    ///     PeerSelectionAutoOrSelf function. Proc_GetCompanyDetail
    /// </summary>
    /// <returns></returns>
    [CustomAuthorization]
    public ActionResult PeerSelectionAutoOrSelf()
    {
        TempData.Peek("BackButtonPeerSel");
        SessionData.PossiblePeerCompanies = string.Empty;
        SessionData.AdditionalPeerCompanies = string.Empty;
        SessionData.FinalPeerCompanies = string.Empty;
        SessionData.SelfPeerCompanies = string.Empty;
        var model = _iPeerAMid.GetCompanyDetails(
            SessionData.BenchmarkCompany.Id!,
            SessionData.BenchmarkCompany.DataYear.ToString());
        if (model != null)
            SessionData.BenchmarkCompany.CopyFrom(model);
        return View(model);
    }

    [CustomAuthorization]
    public ActionResult PeerAutoSuggest()
    {
        // Log.Debug("PeerAutoSuggest");
        const int optionId = 0;
        Log.Info("setting optionid = " + optionId);
        SessionData.IsPossiblePeerCompanies = true;
        SessionData.IsSelfPeerCompanies = false;
        var regions = SessionData.SelectedRegions;
        var list = _iPeerAMid.GetAutoSuggestCompanyList(
            SessionData.Instance.BenchmarkCompany.Id!,
            optionId,
            SessionData.BenchmarkCompany.FinancialYear.ToString(),
            regions);
        dynamic model = new ExpandoObject();
        model.CompanyList = list;
        return View(model);
    }

    /// <summary>
    ///     After click on next button from PeerSelectionAutoOrSelf.cshtml page request redirect to CompanyAutoSelection.cshtml
    ///     page and load data in controllers. Load peer and auto suggestion company data on page via this
    ///     function.Proc_GetFinalListCompanies Proc_GetAutoSuggestedCompanyList
    /// </summary>
    /// <returns></returns>
    [CustomAuthorization]
    public ActionResult CompanyAutoSelection()
    {
        Log.Debug("CompanyAutoSelection");

        //TempData.Peek("BackButtonPeerSel");
        dynamic model = new ExpandoObject();
        SessionData.Instance.PeerSearchResults.Clear();
        //SessionData.Instance.PeerSearchResults = GetSuggestedPeerCompanyList(benchmarkCompanyId, -1, -1, null, null, null, null, null);
        model.AutoList = SessionData.Instance.PeerSearchResults;


        //Log.Debug("Leaving CompanyAutoSelection");
        return View(model);
    }

    private void LoadSessionCompanyList(ICollection<SessionData.CompanyData> cdl, ICollection<Company> cms)
    {
        var count = 0;
        foreach (var m in cms)
        {
            cdl.Add(
                new SessionData.CompanyData
                {
                    Country = m.Country,
                    Id = m.CompanyId,
                    Name = m.CompanyName,
                    Ticker = m.Ticker,
                    Revenue = m.Revenue,
                    IndustryId = m.IndustryId,
                    SubIndustryId = m.SubIndustryId,
                    FinancialYear = m.FinancialYear,
                    DataYear = m.DataYear,
                    IsSuggested = m.IsSuggested,
                    IsSelected = count < 15 && cms.Count <= 15 // new rule: if 16+ companies, select none
                });
            ++count;
        }
    }

    /// <summary>
    ///     Get peer company data for compression. Proc_GetAdditionalPeerCompanies.
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="revenueFrom"></param>
    /// <param name="revenueTo"></param>
    /// <param name="year"></param>
    /// <param name="revenueTo"></param>
    /// <param name="year"></param>
    /// <param name="regions"></param>
    /// <param name="industryFilter"></param>
    /// <param name="subIndustryFilter"></param>
    /// <param name="nameFilter"></param>
    /// <param name="tickerFilter"></param>
    /// <param name="maxCompanies"></param>
    /// <returns></returns>
    [CustomAuthorization]
    [HttpPost]
    [CalledFromExternalCode]
    public JsonResult GetSuggestedPeerCompanies(
        int uid, decimal revenueFrom, decimal revenueTo, int year,
        string? regions,
        string? industryFilter, string? subIndustryFilter,
        string? nameFilter, string? tickerFilter,
        bool bindToSessionData, int maxCompanies)
    {
        try
        {
            //Second Radio Button
            //int OptionId = 0;
            var companies = GetSuggestedPeerCompanyList(
                uid, revenueFrom, revenueTo, year,
                regions,
                industryFilter, subIndustryFilter,
                nameFilter, tickerFilter,
                maxCompanies);


            // Log.Debug("Number of companies: " + mod.Count);
            if (bindToSessionData)
            {
                SessionData.Instance.PeerSearchResults = companies;
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
            var result = Json(new { aaData = companies }, JsonRequestBehavior.AllowGet);
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
    }


    /// <summary>
    ///     Get peer company data for analysis
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="revenueFrom"></param>
    /// <param name="revenueTo"></param>
    /// <param name="year"></param>
    /// <param name="revenueTo"></param>
    /// <param name="year"></param>
    /// <param name="regions"></param>
    /// <param name="industryFilter"></param>
    /// <param name="subIndustryFilter"></param>
    /// <param name="nameFilter"></param>
    /// <param name="tickerFilter"></param>
    /// <param name="maxCompanies"></param>
    /// <returns></returns>
    private List<Company> GetSuggestedPeerCompanyList(
        int uid, decimal revenueFrom, decimal revenueTo, int year,
        string? regions,
        string? industryFilter, string? subIndustryFilter,
        string? nameFilter, string? tickerFilter,
        int maxCompanies)
    {
        // Log.Debug("Entered GetSuggestedPeerCompanyData");
        // Log.Debug($"industryId {industryId}");
        // Log.Debug($"uid {uid}");
        // Log.Debug($"companyName {companyName}");
        // Log.Debug($"revenueStart {revenueStart}");
        // Log.Debug($"revenueTo {revenueTo}");
        // Log.Debug($"year {year}");
        // Log.Debug($"regions {regions}");
        // Log.Debug($"iSortCol0 {iSortCol0}");
        // Log.Debug($"sSortDir0 {sSortDir0}");
        // Log.Debug($"subIndustryId {subIndustryId}");
        // Log.Debug($"bindToSessionData {bindToSessionData}");

        try
        {
            var companies = _iPeerAMid.GetSuggestedPeerCompanyList(
                uid, revenueFrom, revenueTo, year,
                regions,
                industryFilter, subIndustryFilter,
                nameFilter, tickerFilter,
                maxCompanies);
            return companies;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return new List<Company>();
        }
    }




    /// <summary>
    ///     Get peer company data for compression. Proc_GetAdditionalPeerCompanies.
    /// </summary>
    /// <param name="industryId"></param>
    /// <param name="uid"></param>
    /// <param name="companyName"></param>
    /// <param name="revenueStart"></param>
    /// <param name="revenueTo"></param>
    /// <param name="year"></param>
    /// <param name="regions"></param>
    /// <param name="subIndustryId"></param>
    /// <param name="bindToSessionData"></param>
    /// <param name="iSortCol0"></param>
    /// <param name="sSortDir0"></param>
    /// <returns></returns>
    [CustomAuthorization]
    [HttpPost]
    [CalledFromExternalCode]
    public JsonResult GetAdditionalPeerCompanies(string industryId, string uid,
        string companyName, decimal revenueStart, decimal revenueTo, int year,
        string regions, string subIndustryId, bool bindToSessionData,
        int iSortCol0 = 0, string sSortDir0 = "DESC")
    {
        try
        {
            //Second Radio Button
            //int OptionId = 0;
            var companies = GetAdditionalPeerCompanyData(
                industryId,
                uid,
                companyName,
                revenueStart,
                revenueTo,
                year,
                regions ?? "",
                subIndustryId,
                bindToSessionData,
                iSortCol0,
                sSortDir0);

            // Log.Debug("Number of companies: " + mod.Count);
            var result = Json(new { aaData = companies }, JsonRequestBehavior.AllowGet);
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
    }


    /// <summary>
    ///     Get peer company data for analysis
    /// </summary>
    /// <param name="industryId"></param>
    /// <param name="uid"></param>
    /// <param name="companyName"></param>
    /// <param name="revenueStart"></param>
    /// <param name="revenueTo"></param>
    /// <param name="year"></param>
    /// <param name="regions"></param>
    /// <param name="subIndustryId"></param>
    /// <param name="bindToSessionData"></param>
    /// <param name="iSortCol0"></param>
    /// <param name="sSortDir0"></param>
    /// <returns></returns>
    private List<Company> GetAdditionalPeerCompanyData(string industryId, string uid,
        string companyName, decimal revenueStart, decimal revenueTo, int year,
        string regions, string subIndustryId, bool bindToSessionData,
        int iSortCol0 = 0, string sSortDir0 = "DESC")
    {
        Log.Debug("Entered GetAdditionalPeerCompany");
        // Log.Debug($"industryId {industryId}");
        // Log.Debug($"uid {uid}");
        // Log.Debug($"companyName {companyName}");
        // Log.Debug($"revenueStart {revenueStart}");
        // Log.Debug($"revenueTo {revenueTo}");
        // Log.Debug($"year {year}");
        // Log.Debug($"regions {regions}");
        // Log.Debug($"iSortCol0 {iSortCol0}");
        // Log.Debug($"sSortDir0 {sSortDir0}");
        // Log.Debug($"subIndustryId {subIndustryId}");
        Log.Debug($"bindToSessionData {bindToSessionData}");

        try
        {
            //Second Radio Button
            //int OptionId = 0;
            var companies = _iPeerAMid.GetAdditionalPeerCompanyList(
                industryId,
                uid,
                companyName,
                revenueStart,
                revenueTo,
                year,
                regions ?? "",
                iSortCol0,
                sSortDir0,
                SessionData.OptionId,
                subIndustryId);

            if (bindToSessionData)
            {
                SessionData.Instance.PeerSearchResults?.Clear();
                foreach (var company in companies)
                    SessionData.Instance.PeerSearchResults!.Add(company);
            }

            return companies;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return new List<Company>();
        }
    }


    /// <summary>
    /// Get the bench mark company data
    /// Called from 
    /// </summary>
    /// <returns></returns>
    [CustomAuthorization]
    public ActionResult BenchmarkCompanyData()
    {
        dynamic newCompanyFlag = TempData.Peek("BackButtonPeerSel") ?? 0;
        SessionData.PeerSelOption = newCompanyFlag.ToString() == "3" ? 3 : 1;

        var targetId = SessionData.BenchmarkCompany.Id;
        var year = CommonFunc.GetCurrentFy(_iPeerAMid).ToString();

        var clientFunDataModel = ActualDataCollectionCore.GetFilledDataForYear(targetId!, year);
        var model = _iPeerAMid.GetCompanyDetails(targetId!, year);

        dynamic mymodel = new ExpandoObject();
        if (model != null)
        {
            SessionData.BenchmarkCompany.CopyFrom(model);
            mymodel.CompanyModel = model;
        }

        mymodel.ClientDataModel = clientFunDataModel!;
        return View(mymodel);
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    [CustomAuthorization]
    public ActionResult BenchmarkCompanyLatestData()
    {
        return View();
    }


    /// <summary>
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet]
    [CalledFromExternalCode]
    public JsonResult ReallyGetBenchmarkCompanyLatestData(int uid, int year)
    {
        var u = uid.ToString();
        var model = ActualDataCollectionCore.GetFilledDataForYear(u, year.ToString());
        if (model == null)
            return new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        var list = _iPeerAMid.GetCompanyRequiredDataHistory(u);
        dynamic mymodel = new ExpandoObject();
        mymodel.CompanyModel = CostFieldsCalculation.ConvertCurrencyFields(model, model!.DataEntryCurrencySettings)!;
        mymodel.CurrentYear = year;
        mymodel.CompanyList = list!.Where(r => r.Revenue != 0).OrderByDescending(o => o.FinancialYear).ToList();
        return new JsonResult { Data = mymodel, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
    }


    /// <summary>
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost]
    [CalledFromExternalCode]
    public JsonResult SaveActualLatestDataCollectionNew()
    {
        var model = ParseBodyIntoModel();
        //model.CompanyUID = int.Parse(SessionData.BenchmarkCompany.CompanyId);
        //SessionData.BenchmarkCompany.FinancialYear = model.YearId;
        var result =
            ActualDataCollectionCore.SaveActualLatestDataCollection(model);
        model.FinancialYear = model.YearId;
        model.GrossMargin = model.GrossMargin / model.Revenue;
        return new JsonResult { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet }; ;
    }

    /// <summary>
    ///     This function use for load data in ModifyBenchmarkCompanyData.cshtml page. Proc_GetClientActualData
    ///     Proc_GetCompanyRequireData
    /// </summary>
    /// <returns></returns>
    [CustomAuthorization]
    public ActionResult ModifyBenchmarkCompanyData()
    {
        return View("BenchmarkCompanyLatestData");
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    [CustomAuthorization]
    public JsonResult GetBenchmarkCompanyData(string companyId, int year)
    {
        var futureYears = _homeCore.GetFutureYearList();
        if (!futureYears.Contains(year))
            futureYears.Add(year);
        futureYears.Sort();

        var companyYearData = new List<ActualDataCollectionModel>();
        foreach (var y in futureYears)
        {
            var adcm = ActualDataCollectionCore.GetFilledDataForYear(companyId, y.ToString());
            if (adcm != null)
            {
                var converted = CostFieldsCalculation.ConvertCurrencyFields(adcm, adcm.DataEntryCurrencySettings)!;
                //var years = _iPeerAMid.GetCompanyRequiredData(companyId).Where(r => r.Revenue != 0).OrderByDescending(o => o.FinancialYear);
                companyYearData.Add(converted);
            }
        }

        return new JsonResult { Data = new { FutureYears = futureYears, CompanyYears = companyYearData }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
    }


    /// <summary>
    ///     Redirect request on RunAnalysis.cshtml page.
    /// </summary>
    /// <returns></returns>
    [CustomAuthorization]
    public ActionResult RunAnalysis()
    {
        return View();
    }


    /// <summary>
    ///     Update session value.
    /// </summary>
    /// <param name="searchText1"></param>
    /// <param name="searchText2"></param>
    /// <param name="type"></param>
    /// <param name="searchAdditional"></param>
    /// <returns></returns>
    [HttpPost]
    [CustomAuthorization]
    [CalledFromExternalCode]
    public JsonResult SetSessionVariables(string? searchText1, string searchText2, int type,
        string searchAdditional = "")
    {
        searchText1 ??= "";
        searchText2 ??= "";
        searchAdditional ??= "";
        var st1 = searchText1 == null ? "null" : searchText1.Length > 41 ? searchText1.Substring(0, 19) + "..." + searchText1.Substring(searchText1.Length - 19) : searchText1;
        Log.Debug($"SetSessionVariables type = {type}, searchText1 = {st1}, searchText2 = {searchText2}, searchAdditional = {searchAdditional}");
        object returnData = SessionData;
        try
        {
            switch (type)
            {
               default:
                    Log.Error($"Invasalid call to SetSessionVariables {type}");
                    break;

                case 9: // Set Selected Regions
                    SessionData.SelectedRegions = searchAdditional;
                    break;

                case 16: // I hate this
                    // Log.Info(searchText1);
                    if (searchAdditional.Contains("RunAnalysis"))
                    {
                    }
                    else
                    {
                        SessionData.PeerSearchResults!.Clear();
                        if (!string.IsNullOrEmpty(searchText1))
                            SessionData.PeerSearchResults.AddRange(JsonConvert.DeserializeObject<List<Company>>(searchText1!));
                        SessionData.SelectedPeerCompanies.Clear();
                        if (!string.IsNullOrEmpty(searchText2))
                            SessionData.SelectedPeerCompanies.AddRange(JsonConvert.DeserializeObject<List<SessionData.CompanyData>>(searchText2!));
                    }
                    break;


                case 20: // clear for fresh entry onto peer selection page
                    SessionData.PossiblePeerCompanies = string.Empty;
                    SessionData.AdditionalPeerCompanies = string.Empty;
                    SessionData.SelfPeerCompanies = string.Empty;
                    SessionData.FinalPeerCompanies = string.Empty;
                    SessionData.SelectedIndustries = string.Empty;
                    SessionData.SelectedSubIndustries = string.Empty;
                    SessionData.SelectedPeerCompanies.Clear();
                    SessionData.PeerSearchResults.Clear();
                    break;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            returnData = ex;
        }

        // Log.Debug("SetSessionVariables returnData.Type = " + returnData?.GetType() ?? "null");
        return new JsonResult { Data = returnData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
    }


    /// <summary>
    /// Formerly SetSessionVariables(2).
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="year"></param>
    /// <returns></returns>
    [HttpPost]
    [CustomAuthorization]
    [CalledFromExternalCode]
    public void SetBenchmarkCompany(string companyId, int year)
    {
        var u = int.Parse(companyId);
        if ((SessionData.BenchmarkCompany.Uid != u) || (SessionData.BenchmarkCompany.FinancialYear != year))
        {
            SessionData.PossiblePeerCompanies = string.Empty;
            SessionData.AdditionalPeerCompanies = string.Empty;
            SessionData.SelfPeerCompanies = string.Empty;
            SessionData.FinalPeerCompanies = string.Empty;
            SessionData.BenchmarkCompany.Uid = u;
            var company = _iPeerAMid.GetCompanyDetails(u, year);
            company!.CloneInto(SessionData.BenchmarkCompany);
        }
    }


    /// <summary>
    ///     After click on Export the Deliverable button download PPT on browser.
    /// </summary>
    /// <param name="selectedPeers"></param>
    /// <param name="selectedTarget"></param>
    /// <param name="year"></param>
    /// <param name="selectedTargetSymbol"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    [CustomAuthorization]
    [HttpPost]
    [CalledFromExternalCode]
    public JsonResult SavePptReport(string selectedPeers = "", int selectedTarget = 0,
        int year = 0, string selectedTargetSymbol = "",
        PeerAMidService service = PeerAMidService.None)
    {
        if (SessionData.BenchmarkCompany.Uid != selectedTarget)
        {
            var company = _iPeerAMid.GetCompanyDetails(selectedTarget, year);
            SessionData.BenchmarkCompany.CopyFrom(company!);
        }

        var filename = GenerateReport(
            selectedPeers,
            selectedTarget,
            year,
            selectedTargetSymbol,
            service);
        if (string.IsNullOrEmpty(filename))
            return Json(new { }, JsonRequestBehavior.AllowGet);
        // Log.Info("Returning Powerpoint file '" + filename + "'");
        SessionData.PreviousPptFileName = filename;
        return Json(
            new
            {
                FileName = filename
            },
            JsonRequestBehavior.AllowGet);
    }


    /// <summary>
    ///     After click on search button search company data and bind in Company grid list. Proc_GetAllcompanyList
    /// </summary>
    /// <param name="cSearch"></param>
    /// <param name="tSearch"></param>
    /// <param name="regions"></param>
    /// <returns></returns>
    [CustomAuthorization]
    [HttpPost]
    [CalledFromExternalCode]
    public JsonResult GetBenchmarkCompanySearchResults()
    {
        return BenchmarkCompanySearchResults(
            SessionData.cSearch,
            SessionData.tSearch,
            SessionData.SelectedRegions);
    }


    private string? GenerateReport(string selectedPeers, int selectedTarget,
        int year, string selectedTargetSymbol, PeerAMidService service)
    {
        // Log.Info("Entering GenerateReport()");
        try
        {
            Currency? cfs = null;
            Currency? secondChoiceCfs = null;
            foreach (var c in MvcApplication.GlobalStaticData.Currencies)
            {
                if (c.Name == SessionData.BenchmarkCompany.DataEntryCurrency.Trim())
                    cfs = c;
                else if (c.Name == "USD") secondChoiceCfs = c;
            }

            cfs ??= secondChoiceCfs ?? MvcApplication.GlobalStaticData.Currencies[0];

            // Log.Info("Constructing report parameters");
            var parameters = new ReportParameters(
                SessionData,
                selectedPeers,
                selectedTarget,
                year,
                selectedTargetSymbol,
                service,
                cfs,
                SessionData.User.Folder);
            // Log.Info("Starting Powerpoint creation for " + parameters.Service);

            Func<ReportParameters, string> func;
            if (parameters.Service.IsWCD())
                func = CreateWcd;
            else
                func = CreateSga;

            var filename = func(parameters);

            // Log.Info("Returning Powerpoint file '" + filename + "'");
            SessionData.PreviousPptFileName = filename;
            return filename;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return null;
        }
    }

    [AllowAnonymous]
    [HttpGet]
    [CalledFromExternalCode]
    public JsonResult GetGlobalStaticData(string currentVersion)
    {
        if (currentVersion == MvcApplication.GlobalStaticData.CurrentVersion)
            return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
        var s = JsonConvert.SerializeObject(MvcApplication.GlobalStaticData);
        return Json(new { data = s }, JsonRequestBehavior.AllowGet);
    }

    [AllowAnonymous]
    [HttpGet]
    [CalledFromExternalCode]
    public JsonResult GetSessionData()
    {
        return Json(SessionData, JsonRequestBehavior.AllowGet);
    }


    [AllowAnonymous]
    [HttpPost]
    [CalledFromExternalCode]
    public JsonResult GetCompanyDetailsByName(string companyName, string year)
    {
        try
        {
            var company = _iPeerAMid.GetCompanyDetailsByName(companyName, year);
            Log.Info("Company fetched: " + company?.Name);
            return Json(company, JsonRequestBehavior.AllowGet);
        }
        catch (Exception e)
        {
            Log.Error(e);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    [AllowAnonymous]
    [HttpPost]
    [CalledFromExternalCode]
    public JsonResult GetCompanyDetailsByTicker(string ticker, string year)
    {
        try
        {
            var company = _iPeerAMid.GetCompanyDetailsByName(ticker, year);
            Log.Info("Company fetched: " + company?.Name);
            return Json(company, JsonRequestBehavior.AllowGet);
        }
        catch (Exception e)
        {
            Log.Error(e);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    public ActionResult ContactUs()
    {
        return View();
    }

    /// <summary>
    ///     Bind configuration year data in application by Proc_GetCurrentFinancialYear.
    /// </summary>
    /// <returns></returns>
    [CustomAuthorization(AdminOnly = true)]
    public ActionResult ConfigManager()
    {
        var appSetting = _iPeerAMid.GetCurrentFinancialData();
        return View(appSetting);
    }

    [CustomAuthorization(AdminOnly = true)]
    [HttpPost]
    public ActionResult ConfigManager(ApplicationConfig appSetting)
    {
        try
        {
            _iPeerAMid.UpdateCurrentFinancialYear(appSetting);
            appSetting.Message = "Fiscal Year is saved successfully.";
            return View(appSetting);
        }
        catch (Exception /*ex*/)
        {
            appSetting.Message = "Something went wrong!";
            return View(appSetting);
        }
    }

    /// <summary>
    ///     Custom logic what would the position of pointing star in chart.
    /// </summary>
    /// <param name="min"></param>
    /// <param name="p25"></param>
    /// <param name="p50"></param>
    /// <param name="p75"></param>
    /// <param name="max"></param>
    /// <param name="starVal"></param>
    /// <returns></returns>
    public decimal DemographicStarLocation(decimal min, decimal p25, decimal p50, decimal p75, decimal max,
        decimal starVal)
    {
        decimal fixPercentage;
        decimal upperValue;
        decimal lowerValue;
        decimal sectionDifference = 25;

        if (starVal < min) return 0;
        if (starVal <= p25)
        {
            fixPercentage = 0;
            upperValue = p25;
            lowerValue = min;
        }
        else if (starVal <= p50)
        {
            fixPercentage = 25;
            upperValue = p50;
            lowerValue = p25;
        }
        else if (starVal <= p75)
        {
            fixPercentage = 50;
            upperValue = p75;
            lowerValue = p50;
        }
        else if (starVal <= max)
        {
            fixPercentage = 75;
            upperValue = max;
            lowerValue = p75;
        }
        else
        {
            return 0;
        }

        var rangeDifference = upperValue - lowerValue;
        if (rangeDifference == 0) return 0;
        return fixPercentage + (starVal - lowerValue) * (sectionDifference / rangeDifference);
    }

    public double DemographicStarLocation(double min, double p25, double p50, double p75, double max, double starVal)
    {
        const double sectionDifference = 25;

        var e = max - min;
        if (Math.Abs(e) < 0.000001)
            return max;
        if (e > 0)
        {
            if (starVal < min) return 0;
            double fixPercentage;
            double upperValue;
            double lowerValue;
            if (starVal <= p25)
            {
                fixPercentage = 0;
                upperValue = p25;
                lowerValue = min;
            }
            else if (starVal <= p50)
            {
                fixPercentage = 25;
                upperValue = p50;
                lowerValue = p25;
            }
            else if (starVal <= p75)
            {
                fixPercentage = 50;
                upperValue = p75;
                lowerValue = p50;
            }
            else if (starVal <= max)
            {
                fixPercentage = 75;
                upperValue = max;
                lowerValue = p75;
            }
            else
            {
                return 100;
            }

            var rangeDifference = upperValue - lowerValue;
            if (rangeDifference == 0) return 0;
            return fixPercentage + (starVal - lowerValue) * (sectionDifference / rangeDifference);
        }

        return DemographicStarLocation(max, p75, p50, p25, min, starVal);
    }

    // Generate a guaranteed-unique filename for the report.
    // Returns only the filename without an extension -- no path
    private static string GenerateDeliverableFileName(ReportParameters pptParameters, string extension)
    {
        var companySymbol = pptParameters.SelectedTargetSymbol;
        if (string.IsNullOrWhiteSpace(companySymbol))
        {
            companySymbol = ""; // should not ever happen
        }
        else
        {
            var colon = companySymbol.LastIndexOf(':');
            if (colon >= 0)
                companySymbol = companySymbol.Substring(colon + 1);
        }

        var now = DateTime.Now;
        string baseName;

        if (pptParameters.Service.IsWCD())
            baseName = companySymbol + "_AM_WC_Diagnostics_" + now.ToString("dd-MMM-yyyy") + "_";
        else
            baseName = companySymbol + "_AM_SGA_Diagnostics_" + now.ToString("dd-MMM-yyyy") + "_";

        var index = 1;
        while (index < 100)
        {
            var name = baseName + index.ToString("D2");
            if (!System.IO.File.Exists(pptParameters.OutputFolder + "\\" + name + extension))
                return name;
            ++index;
        }

        return baseName + Guid.NewGuid();
    }

    private class ReportParameters
    {
        public readonly string BenchmarkCompanyDisplayName;
        public readonly string BenchmarkCompanyName;
        public readonly Currency Currency;
        public readonly double ExchangeRate;
        public readonly string Industry;
        private int _optionId;
        public int OptionId
        {
            get { /*Log.Info("Getting optionid = " + _optionId);*/ return _optionId; }
            set { /*Log.Info("Setting optionid = " + value);*/ _optionId = value; }
        }
        public readonly string OutputFolder = "";
        public readonly decimal Revenue;
        public readonly string SelectedPeers;
        public readonly int SelectedTarget;

        //public readonly string SelectedTargetName;
        public readonly string SelectedTargetSymbol;

        public readonly PeerAMidService Service;
        public readonly string SubIndustry;
        public readonly string SubIndustryName;

        //public readonly string TabRandomPDFId;
        //public readonly string UserId;
        //public readonly string UserName;

        public readonly int Year;

        public ReportParameters(SessionData sd, string selectedPeers, int selectedTarget,
            int year, string selectedTargetSymbol,
            PeerAMidService service, Currency cfs,
            string? outputFolder = null)
        {
            OptionId = sd.OptionId;
            Industry = sd.BenchmarkCompany.GetIndustry()!;
            SubIndustry = sd.BenchmarkCompany.GetSubIndustry()!;
            SubIndustryName = sd.BenchmarkCompany.GetSubIndustryName()!;
            BenchmarkCompanyName = sd.BenchmarkCompany.Name!;
            ExchangeRate = sd.BenchmarkCompany.DataEntryExchangeRate;
            BenchmarkCompanyDisplayName = sd.BenchmarkCompany.DisplayName!;
            Revenue = sd.BenchmarkCompany.Revenue;
            //UserName = sd.User.Name;

            SelectedPeers = selectedPeers;
            SelectedTarget = selectedTarget;
            Year = year;
            SelectedTargetSymbol = selectedTargetSymbol;
            Service = service;
            Currency = cfs;
            OutputFolder = outputFolder ?? SessionData.Instance.User.Folder ?? "";
        }

        //public string ReportTitle => Service.GetReportTitle();
    }


    #region Test -- loads up given companies and goes strait to the analysis

    [CustomAuthorization(AdminOnly = true)]
    public ActionResult Test()
    {
        try
        {
            var url = Request.Url.ToString();
            var index = url.IndexOf('?');
            if (index < 0)
                throw new Exception();
            var allParameters = url.Substring(index + 1);
            var parameters = new List<string>();
            var position = 0;
            while (position < allParameters.Length)
            {
                while (position < allParameters.Length && char.IsWhiteSpace(allParameters[position]))
                    ++position;
                if (position < allParameters.Length)
                {
                    var c = allParameters[position];
                    if (char.IsWhiteSpace(c))
                    {
                        ++position;
                    }
                    else if (c is '\'' or '\"')
                    {
                        var start = position++;
                        while (position < allParameters.Length && allParameters[position] != c)
                        {
                            if (allParameters[position] == '\\')
                                ++position;
                            position++;
                        }

                        if (position < allParameters.Length)
                        {
                            parameters.Add(allParameters.Substring(start, position - start));
                            ++position;
                            while (position < allParameters.Length && char.IsWhiteSpace(allParameters[position]))
                                ++position;
                        }
                        else
                        {
                            parameters.Add(allParameters.Substring(start));
                        }

                        if (position < allParameters.Length && allParameters[position] == ',')
                            ++position;
                    }
                    else
                    {
                        var start = position++;
                        while (position < allParameters.Length && allParameters[position] != ',')
                            position++;
                        parameters.Add(allParameters.Substring(start, position - start));
                        ++position;
                    }
                }
            }

            return Test(parameters);
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return Redirect("/");
        }
    }


    private ActionResult Test(List<string> parameters)
    {
        if (parameters.Count < 3)
            throw new Exception();

        var type = parameters[0];
        if (type == "SGA")
            SessionData.SelectedService = PeerAMidService.SgaFull;
        else if (type == "HLD")
            SessionData.SelectedService = PeerAMidService.SgaShort;
        else if (type == "WCD")
            SessionData.SelectedService = PeerAMidService.WcdFull;
        else if (type == "CCC")
            SessionData.SelectedService = PeerAMidService.WcdShort;
        else
            throw new Exception();

        SessionData.OptionId = 1;

        var benchmarkCompany = FindCompanyForTest(parameters[1]);
        SessionData.BenchmarkCompany.Id = benchmarkCompany.Id;
        SessionData.BenchmarkCompany.FinancialYear = benchmarkCompany.FinancialYear;
        SessionData.BenchmarkCompany.IndustryId = benchmarkCompany.IndustryId;
        SessionData.BenchmarkCompany.SubIndustryId = benchmarkCompany.SubIndustryId;
        SessionData.BenchmarkCompany.Name = benchmarkCompany.Name;
        SessionData.BenchmarkCompany.CompanyNameMixedCase = benchmarkCompany.CompanyNameMixedCase;
        SessionData.BenchmarkCompany.ShortNameMixedCase = benchmarkCompany.ShortNameMixedCase;
        SessionData.BenchmarkCompany.DisplayName = benchmarkCompany.DisplayName;
        SessionData.PossiblePeerCompanies = string.Empty;
        SessionData.AdditionalPeerCompanies = string.Empty;
        SessionData.BenchmarkCompany.Ticker = benchmarkCompany.Ticker;

        var peers = new SortedList<string, Company>();
        for (var i = 2; i < parameters.Count; i++)
        {
            var c = FindCompanyForTest(parameters[i], null);
            if (c != null && !peers.ContainsKey(c.Id!))
                peers.Add(c.Id!, c);
        }

        if (peers.Count < 6)
            throw new Exception();

        var peersText = string.Join(",", peers.Keys);
        SessionData.FinalPeerCompanies = peersText;
        SessionData.SelfPeerCompanies = "";

        return View("RunAnalysis");
    }

    /*
    private Company FindBenchmarkCompanyForTest(string company)
    {
        var companies = _iPeerAMid.GetBenchmarkCompanyList(company, null, null, "0");
        if (companies.Count == 0)
            companies.AddRange(_iPeerAMid.GetBenchmarkCompanyList(null, company, null, "0"));
        if (companies.Count == 0)
            throw new Exception();
        return companies[0];
    }
    */

    private Company FindCompanyForTest(string search, string? year = null)
    {
        var numeric = true;
        foreach (var c in search)
        {
            if (!char.IsDigit(c))
            {
                numeric = false;
                break;
            }
        }

        var company = (numeric ? _iPeerAMid.GetCompanyDetails(search, year) : _iPeerAMid.GetCompanyDetailsByName(search, year)) ?? _iPeerAMid.GetCompanyDetailsByTicker(search, year);
        return company ?? throw new Exception();
    }

    #endregion

#if false
    #region WCAP -- generate SK's WCAP figures

    [CustomAuthorization(AdminOnly = true)]
    public ActionResult Wcap() //  /Wcap?year,target,uid,uid,uid...
    {
        try
        {
            var url = Request.Url!.ToString();
            var index = url.IndexOf('?');
            if (index < 0)
                throw new Exception();
            var parameters = url.Substring(index + 1).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return Wcap(parameters);
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return Redirect("/");
        }
    }


    private ActionResult Wcap(string[] parameters)
    {
        var year = parameters[0];
        var companies = new List<Dictionary<string, string>>();
        for (var i = 1; i < parameters.Length; ++i)
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetSqlStringCommand($"SELECT * FROM YS.View_AllCompanyFactsData WHERE UID='{parameters[i]}'");
            Log.Info(dbCmd);
            using var reader = db.ExecuteReader(dbCmd);
            if (reader != null)
            {
                reader.Read();
                Dictionary<string, string> company = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                for (var j = 0; j < reader.FieldCount; ++j)
                {
                    var name = reader.GetName(j);

                    if (reader.IsDBNull(j))
                    {
                        company[name] = "null";
                    }
                    else
                    {
                        var value = reader.GetValue(j);
                        if (value is string s)
                            company[name] = "\"" + s + "\"";
                        else if (value is DateTime d)
                            company[name] = "\"" + d + "\"";
                        else
                            company[name] = value.ToString();
                    }
                }

                companies.Add(company);
            }
        }

        var folder = ConfigurationManager.AppSettings.GetForThisMachine("LogFolder");
        var filename = Path.Combine(folder, "WCAP " + DateTime.Now.ToString("yyyy MMdd HHmmss") + ".csv");
        using var file = System.IO.File.CreateText(filename);

        try
        {
            string[] fieldNames1 = new[] { "UID", "YSID", "SIC_Code", "coname", "CIK_ID", "US1K", "Form", "FYE", "Ticker", "Country", "ExRate", "Currency", "CreatedBy", "CreatedOn", "UpdatedBy", "UpdatedOn", "CostUnit" };
            var widths1 = FindWidths(companies, fieldNames1);
            Headers(file, fieldNames1, widths1);
            foreach (Dictionary<string, string> company in companies) Csv(file, company, fieldNames1, widths1);

            file.WriteLine();

            string[] fieldNames2 = new[]
            {
                "FACTID", "coname", "UID", "SIC2D", "SIC_Code", "SGA_MULT", "YSCID", "CIK_ID", "Rev1", "SGA1",
                "GA1M", "SGM1", "EE", "RevPerEE", "Cash_Equiv", "Curr_Assets", "Total_Assets", "Curr_Liab",
                "COGS", "AR", "Inventory", "AP", "T_Liab", "Total_Equity", "MKTCAP", "TEQLIAB", "RetainedE",
                "WCTA", "CASHCL", "CASHTA", "DSO", "DIO", "DPO", "CCC",
                "EBITDA1", "EM1", "GP", "NI", "EBIT", "GPPE", "NPPE", "ROA", "WCAP", "WCAP1",
                "WCSALES", "AvgWorkingCapital", "NetSalesPerAWC", "ITURNS", "FATURN", "TATURN",
                "EQTURNS", "Invsales", "RTURNS", "QASALES", "CASALES", "DBTEQ", "DBTTA", "FNMULTI",
                "FAEQLTL", "RETA",
                "CreatedBy", "CreatedOn", "UpdatedBy", "UpdatedOn", "DataYear",
                "OneDaySales", "OneDayCOGS",
                "ActualRoche", "CapitalEmployed"
            };

            var widths2 = FindWidths(companies, fieldNames2);
            Headers(file, fieldNames2, widths2);
            foreach (Dictionary<string, string> company in companies) Csv(file, company, fieldNames2, widths2);
        }
        catch (Exception ex)
        {
            file.WriteLine(ex.ToString());
            file.WriteLine(ex.StackTrace);
        }

        //foreach (var kvp in companies[0])
        //    file.WriteLine(kvp.Key);


        return Redirect("/");

        void Headers(StreamWriter writer, string[] fields, int[] widths)
        {
            for (var k = 0; k < fields.Length; ++k)
            {
                writer.Write("\"" + fields[k] + "\"");
                if (k < fields.Length - 1)
                    writer.Write(",");
                for (var m = fields[k].Length + 1; m < widths[k]; ++m)
                    writer.Write(' ');
            }

            writer.WriteLine();
        }

        void Csv(StreamWriter writer, Dictionary<string, string> comp, string[] fields, int[] widths)
        {
            for (var k = 0; k < fields.Length; ++k)
            {
                try
                {
                    var data = comp[fields[k]];
                    writer.Write(data);
                    if (k < fields.Length - 1)
                        writer.Write(", ");
                    for (var m = data.Length; m < widths[k]; ++m)
                        writer.Write(' ');
                }
                catch (Exception x)
                {
                    throw new Exception("Bad name " + fields[k], x);
                }
            }

            writer.WriteLine();
        }

        int[] FindWidths(List<Dictionary<string, string>> comps, string[] fields)
        {
            var widths = new int[fields.Length];
            for (var k = 0; k < fields.Length; ++k)
            {
                widths[k] = fields[k].Length + 2; // quote marks
                foreach (Dictionary<string, string> comp in comps)
                {
                    try
                    {
                        var data = comp[fields[k]];
                        if (data.Length > widths[k])
                            widths[k] = data.Length;
                    }
                    catch (Exception x)
                    {
                        throw new Exception("Bad name " + fields[k], x);
                    }
                }
            }

            return widths;
        }
    }

    #endregion
#endif
}
