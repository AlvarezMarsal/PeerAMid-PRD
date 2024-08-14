// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace

using PeerAMid.Business;
using PeerAMid.Data;

#nullable enable

namespace PeerAMid.Core;

public interface IHomeCore
{
    //List<IndustryModel>? GetIndustryList(string userId);

    List<IndustryModel> GetSICIndustryCompanyList(string phase, int unusedOptionId, int max);

    List<UserCompanyDetails> GetCompanyList(string phase, int optionID, int max);

    UserCompanyDetailsList GetCompanyNames(string phrase, bool byTicker, int max, string? tag = null);

    List<SubIndustryModel>? GetSubIndustryList(int industryId = 0, string? subIndustryText = null);

    List<PeerCompanyModel> GetPeerCompanyList(string industryId, string subIndustryText);

    DemographicModel GetDemographicChartData(int targetCompanyId, string subIndustryText, int year, int optionId);

    SGAModel GetSgaChartData(int targetCompanyId, string targetCompanySymbol, string subIndustryText, int year,
        int optionId, bool requireMatchingFiscalYear = false);

    WorkingCapitalData GetWorkingCapitalData(int targetCompanyId, string targetCompanySymbol,
        string selectedPeerList, int year, int optionId,
        bool requireMatchingFiscalYear = false);

    WorkingCapitalTrendData GetWorkingCapitalTrendData(int targetCompanyId, string selectedPeerList, int firstYear,
        int lastYear, int maxYears);

    SGAPerformanceModel GetPerformanceChartData(int targetCompanyId, string selectedPeerList, int year,
        int optionId);

    FunctionalRevenueModel GetFunCostAsPercentOfRevenueData(int targetCompanyId, string selectedPeerList, int year,
        int optionId);

    FunctionalRevenueModel GetFTEDecomposedCostAsPercentOfRevenueData(
        int targetCompanyId, string selectedPeerList, int year, int optionId);

    List<Company> GetFinalPeerAndTargetCompaniesList(int targetCompanyId, string selectedPeerList, int year);

    SGAWaterfallModel GetDecomposedWaterFallMedian(int targetCompanyId, string selectedPeerList, int year,
        int optionId);

    SGAWaterfallModel GetDecomposedWaterFallTopQ(int targetCompanyId, string selectedPeerList, int year,
        int optionId);

    SGAWaterfallModel GetDecomposedWaterFallTopD(int targetCompanyId, string selectedPeerList, int year,
        int optionId);

    AppPhrases GetAppPhrases(AppPhrases? currentAppPhrases, string app, string? language);

    List<int> GetFutureYearList();
}
