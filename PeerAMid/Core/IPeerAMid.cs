using PeerAMid.Business;
using PeerAMid.Data;
using System.Data;

#nullable enable
// ReSharper disable once CheckNamespace
namespace PeerAMid.Core;

public interface IPeerAMidCore
{
    List<Company> GetAutoSuggestCompanyList(string companyId, int optionId, string year, string? regions);

    List<Company> GetBenchmarkCompanyList(string cSearch, string tSearch, string? regions, string year);

    Company? GetCompanyDetails(int companyId, int? year);
    Company? GetCompanyDetails(string companyId, string? year);
    Company? GetCompanyDetailsByName(string companyName, string? year);
    Company? GetCompanyDetailsByTicker(string ticker, string? year);

    List<Company> GetAdditionalPeerCompanyList(string industryId, string uid, string companyName,
        decimal revenueStart, decimal revenueTo, int year,
        string regions,
        int iSortCol0, string sSortDir0, int optionId, string subIndustryId,
        string? ticker = null);

    List<Company> GetSuggestedPeerCompanyList(
        int uid, decimal revenueFrom, decimal revenueTo, int year,
        string? regions,
        string? industryFilter, string? subIndustryFilter,
        string? nameFilter, string? tickerFilter,
        int maxCompanies);

    List<Company> GetFinalPeersCompanyList(string possiblePeer, string selfPeer, string year, int optionId, string? regions);

    List<Company> GetPreviouslySelectedPeers(string companyId);

    List<Company> GetCompanyRequiredData(string companyId);

    ActualDataCollectionModel GetClientFunctionalData(string companyId, string year);

    int SaveRunAnalysis(string filename, string details);

    DataTable GetMinMaxRevenueOfCurrentYear();

    int GetCurrentFinancialYear();

    ApplicationConfig GetCurrentFinancialData();

    int UpdateCurrentFinancialYear(ApplicationConfig appSetting);

    List<Company> GetCompanyRequiredDataHistory(string companyId);


}
