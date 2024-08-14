using System.Data;

namespace PeerAMid.DataAccess;

public interface IPeerAMidDataAccess
{
    IDataReader GetAutoSuggestCompanyList(string companyId, int optionId, string year, string regions);

    IDataReader GetBenchmarkCompanyList(string cSearch, string tSearch, string? regions, string year);

    IDataReader GetCompanyDetails(string companyId, string? year);

    IDataReader GetAdditionalPeerCompanyList(string industryId, string uid, string companyName,
        decimal revenueStart, decimal revenueTo, int year,
        string regions,
        int iSortCol0, string sSortDir0,
        int optionId, string subIndustryId, string? ticker = null);

    IDataReader GetSuggestedPeerCompanyList(
        int uid, decimal revenueFrom, decimal revenueTo, int year,
        string? regions,
        string? industryFilter, string? subIndustryFilter,
        string? nameFilter, string? tickerFilter,
        int maxCompanies);


    IDataReader GetFinalPeersCompanyList(string possiblePeer, string selfPeer, string year, int optionId, string regions);

    IDataReader GetPreviouslySelectedPeers(string companyId);

    IDataReader GetCompanyRequiredData(string companyId);

    IDataReader GetClientFunctionalData(string companyId, string year);

    int SaveRunAnalysis(string filename, string details);

    DataTable GetMinMaxRevenueOfCurrentYear();

    IDataReader GetCurrentFinancialYear();

    int UpdateCurrentFinancialYear(int settingId, int year);

    IDataReader GetCompanyRequiredDataHistory(string companyId);

    IDataReader GetCompanyDetailsByName(string companyName, string? year);
    IDataReader GetCompanyDetailsByTicker(string ticker, string? year);
}
