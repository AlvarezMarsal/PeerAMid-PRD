using PeerAMid.Business;
using PeerAMid.Core;
using PeerAMid.Data;
using PeerAMid.Support;
using PeerAMid.Utility;
using System;
using System.Collections.Generic;
using System.Web.Http;

#nullable enable

namespace YardStickPortal;

//[AuthorizationPrivilegeFilter]
public class HomeApiController : ApiController
{
    private readonly IHomeCore _homeCore;
    private readonly IPeerAMidCore _iPeerAMid;
    private static SessionData SessionData => SessionData.Instance;


    public HomeApiController(IHomeCore homeCore, IPeerAMidCore iPeerAMid)
    {
        _homeCore = homeCore;
        _iPeerAMid = iPeerAMid;
    }


    public Company? GetBenchmarkCompanyDetails()
    {
        var company = _iPeerAMid.GetCompanyDetails(SessionData.BenchmarkCompany.Id!, SessionData.BenchmarkCompany.DataYear.ToString());
        if (company != null)
            SessionData.BenchmarkCompany.CopyFrom(company);
        return company;
    }

    public List<IndustryModel> GetSicIndustryCompanyList(string phrase, int optionId, int max = 0)
    {
        Log.Info("optionid = " + optionId);
        return _homeCore.GetSICIndustryCompanyList(phrase, optionId, max);
        // Same for SGA & WCD
    }

    /// <summary>
    ///     Get company list data on page. Proc_GetIndustryAndCompanyList
    /// </summary>
    /// <param name="phrase"></param>
    /// <param name="userId"></param>
    /// <param name="optionId"></param>
    /// <returns></returns>
    public List<UserCompanyDetails> GetCompanyList(string phrase, int optionId, int max = 0)
    {
        Log.Info("optionid = " + optionId);
        return _homeCore.GetCompanyList(phrase, optionId, max);
        // Same for SGA & WCD
    }

    /// <summary>
    /// </summary>
    /// <param name="phrase"></param>
    /// <param name="byTicker"></param>
    /// <param name="max"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public UserCompanyDetailsList GetCompanyNames(string phrase, bool byTicker, int max, string? tag = null)
    {
        return _homeCore.GetCompanyNames(phrase, byTicker, max, tag);
    }

    /// <summary>
    ///     Load sub industry data in dropdown. 
    /// </summary>
    /// <param name="industryId"></param>
    /// <param name="phrase"></param>
    /// <returns></returns>
    public List<SubIndustryModel>? GetSubIndustryList(int industryId, string? phrase = null)
    {
        return _homeCore.GetSubIndustryList(industryId, phrase);
        // Same for SGA & WCD
    }

    /// <summary>
    ///     Load year data in dropdown. 
    /// </summary>
    /// <returns></returns>
    [CalledFromExternalCode]
    public List<int> GetFutureYearList()
    {
        return _homeCore.GetFutureYearList();
    }

    public List<PeerCompanyModel> GetPeerCompanyList(string industryId, string subIndustryList)
    {
        return _homeCore.GetPeerCompanyList(industryId, subIndustryList);
        // Same for SGA & WCD
    }

    /// <summary>
    ///     Peer Group* Demographics
    /// </summary>
    /// <param name="targetCompanyId"></param>
    /// <param name="selectedPeerList"></param>
    /// <param name="year"></param>
    /// <param name="optionId"></param>
    /// <returns></returns>
    /// Only used by SG&A
    [HttpGet]
    [CalledFromExternalCode]
    public DemographicModel GetDemographicChartData(int targetCompanyId, string selectedPeerList, int year,
        int optionId)
    {
        _iPeerAMid.SaveRunAnalysis("", selectedPeerList);
        Log.Info("optionid = " + optionId);
        // Log.Debug("Calling _homeCore.GetDemographicChartData");
        try
        {
            return _homeCore.GetDemographicChartData(targetCompanyId, selectedPeerList, year, optionId);
        }
        catch (Exception e)
        {
            Log.Error(e);
            return new DemographicModel();
        }
    }

    /// <summary>
    ///     SG&A Cost as a Percent of Revenue*
    /// </summary>
    /// <param name="targetCompanyId"></param>
    /// <param name="targetCompanySymbol"></param>
    /// <param name="selectedPeerList"></param>
    /// <param name="year"></param>
    /// <param name="optionId"></param>
    /// <param name="requireMatchingFiscalYear"></param>
    /// <returns></returns>
    [HttpGet]
    [CalledFromExternalCode]
    public SGAModel GetSgaChartData(int targetCompanyId, string targetCompanySymbol, string selectedPeerList, int year,
        int optionId, bool requireMatchingFiscalYear = false)
    {
        Log.Info("optionid = " + optionId);
        var model = _homeCore.GetSgaChartData(targetCompanyId, targetCompanySymbol, selectedPeerList, year, optionId, requireMatchingFiscalYear);
        return model;
    }

    /// <summary>
    ///     Various WCD data
    /// </summary>
    /// <param name="targetCompanyId"></param>
    /// <param name="targetCompanySymbol"></param>
    /// <param name="selectedPeerList"></param>
    /// <param name="year"></param>
    /// <param name="optionId"></param>
    /// <param name="userId"></param>
    /// <param name="requireMatchingFiscalYear"></param>
    /// <returns></returns>
    [HttpGet]
    [CalledFromExternalCode]
    public WorkingCapitalData GetWorkingCapitalData(int targetCompanyId, string targetCompanySymbol,
        string selectedPeerList, int year, int optionId,
        bool requireMatchingFiscalYear = false)
    {
        Log.Info("optionid = " + optionId);
        // Log.Debug("Entered GetWorkingCapitalData");
        try
        {
            var wcd = _homeCore.GetWorkingCapitalData(
                targetCompanyId,
                targetCompanySymbol,
                selectedPeerList,
                year,
                optionId,
                requireMatchingFiscalYear);
            // Log.Debug("Leaving GetWorkingCapitalData");
            return wcd;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            // Log.Debug("Leaving GetWorkingCapitalData");
            return new WorkingCapitalData(new SelectedTargetAndSubIndustriesModel());
        }
    }

    /// <summary>
    ///     Various WCD data
    /// </summary>
    /// <param name="targetCompanyId"></param>
    /// <param name="targetCompanySymbol"></param>
    /// <param name="selectedPeerList"></param>
    /// <param name="year"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    [HttpGet]
    public WorkingCapitalTrendData GetWorkingCapitalTrendData(int targetCompanyId, string targetCompanySymbol,
        string selectedPeerList, int firstYear, int lastYear,
        int maxYears)
    {
        // Log.Debug("Entered GetWorkingCapitalTrendData");
        try
        {
            var wcd = _homeCore.GetWorkingCapitalTrendData(
                targetCompanyId,
                selectedPeerList,
                firstYear,
                lastYear,
                maxYears);
            // Log.Debug("Leaving GetWorkingCapitalTrendData");
            return wcd;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            // Log.Debug("Leaving GetWorkingCapitalTrendData");
            return new WorkingCapitalTrendData(0, 0);
        }
    }

    /// <summary>
    ///     SG&A vs. EBITDA Performance*
    /// </summary>
    /// <param name="targetCompanyId"></param>
    /// <param name="selectedPeerList"></param>
    /// <param name="year"></param>
    /// <param name="optionId"></param>
    /// <returns></returns>
    [HttpGet]
    [CalledFromExternalCode]
    public SGAPerformanceModel GetSgaPerformanceChartData(int targetCompanyId, string selectedPeerList, int year,
        int optionId)
    {
        Log.Info("optionid = " + optionId);
        var model = _homeCore.GetPerformanceChartData(targetCompanyId, selectedPeerList, year, optionId);
        return model;
    }

    /// <summary>
    ///     SG&A Decomposed Functional Costs Analysis*
    /// </summary>
    /// <param name="targetCompanyId"></param>
    /// <param name="selectedPeerList"></param>
    /// <param name="year"></param>
    /// <param name="optionId"></param>
    /// <returns></returns>
    [HttpGet]
    [CalledFromExternalCode]
    public FunctionalRevenueModel GetFunCostAsPercentOfRevenueData(int targetCompanyId, string selectedPeerList,
        int year, int optionId)
    {
        Log.Info("optionid = " + optionId);
        var model = _homeCore.GetFunCostAsPercentOfRevenueData(targetCompanyId, selectedPeerList, year, optionId);
        return model;
    }

    /// <summary>
    ///     SG&A Decomposed Functional FTE Analysis*.
    /// </summary>
    /// <param name="targetCompanyId"></param>
    /// <param name="selectedPeerList"></param>
    /// <param name="year"></param>
    /// <param name="optionId"></param>
    /// <returns></returns>
    [HttpGet]
    [CalledFromExternalCode]
    public FunctionalRevenueModel GetFteDecomposedCostAsPercentOfRevenueData(
        int targetCompanyId, string selectedPeerList, int year, int optionId)
    {
        Log.Info("optionid = " + optionId);
        var model = _homeCore.GetFTEDecomposedCostAsPercentOfRevenueData(
            targetCompanyId,
            selectedPeerList,
            year,
            optionId);
        return model;
    }

    /// <summary>
    ///     Estimated Opportunity Waterfall* (quartile)
    ///     Estimated Opportunity Waterfall*(Median)
    ///     Estimated Opportunity Waterfall* (Decile)
    /// </summary>
    /// <param name="targetCompanyId"></param>
    /// <param name="selectedPeerList"></param>
    /// <param name="year"></param>
    /// <param name="optionId"></param>
    /// <returns></returns>
    [HttpGet]
    [CalledFromExternalCode]
    public DecomposedSgaWaterFallData GetDecomposedSgaWaterFall(int targetCompanyId, string selectedPeerList = "", int year = 0,
        int optionId = 0)
    {
        var data = new DecomposedSgaWaterFallData();

        try
        {
            data.TopQChartData = _homeCore.GetDecomposedWaterFallTopQ(targetCompanyId, selectedPeerList, year, optionId);
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }

        try
        {
            data.MedianChartData = _homeCore.GetDecomposedWaterFallMedian(targetCompanyId, selectedPeerList, year, optionId);
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }

        try
        {
            data.TopDChartData = _homeCore.GetDecomposedWaterFallTopD(targetCompanyId, selectedPeerList, year, optionId);
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }

        return data;
    }
}
