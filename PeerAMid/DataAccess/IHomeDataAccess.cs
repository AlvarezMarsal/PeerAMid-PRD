// ReSharper disable InconsistentNaming
using System.Data;

#nullable enable

namespace PeerAMid.DataAccess;

public interface IHomeDataAccess
{
    //IDataReader GetIndustryList(string userId);

    IDataReader GetSubIndustryList(string? industryId, string? subIndustryText);

    IDataReader GetSICIndustryCompanyList(string phase, int optionID, int max);

    IDataReader GetPeerCompanyList(string industryId, string subIndustryList);

    IDataReader GetDemographicChartData(int targetCompanyId, string selectedPeerList, int year, int optionId);

    IDataReader GetSGAChartData(int targetCompanyId, string selectedPeerList, int year, int optionId, bool requireMatchingFiscalYear = false);

    IDataReader GetWorkingCapitalData(int targetCompanyId, string selectedPeerList, int year, int optionId,
        bool requireMatchingFiscalYear = false);

    IDataReader GetWorkingCapitalTrendData(int targetCompanyId, string selectedPeerList, int firstYear,
        int lastYear);

    IDataReader GetSelectedTargetAndSubIndustriesModel(string subIndustoryList);

    IDataReader GetSGAPerformanceChartData(int targetCompanyId, string selectedPeerList, int year, int optionId);

    IDataReader GetFunCostAsPercentOfRevenueData(int targetCompanyId, string selectedPeerList, int year,
        int optionId);

    IDataReader GetFTEDecomposedCostAsPercentOfRevenueData(int targetCompanyId, string selectedPeerList, int year,
        int optionId);

    IDataReader GetFinalPeerAndTargetCompaniesList(int targetCompanyId, string selectedPeerList, int year);

    IDataReader GetDecomposedSGAWaterFallTopQ(int targetCompanyId, string selectedPeerList, int year, int optionId);

    IDataReader GetDecomposedSGAWaterFallMedian(int targetCompanyId, string selectedPeerList, int year,
        int optionId);

    IDataReader GetDecomposedSGAWaterFallTopD(int targetCompanyId, string selectedPeerList, int year, int optionId);

    IDataReader GetFutureYears();

    IDataReader GetAppPhrases(string app, string language, long versionCutoff);
}
