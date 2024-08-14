using Microsoft.Practices.EnterpriseLibrary.Data;
using PeerAMid.Data;
using PeerAMid.Support;
using System.Data;
using System.Data.Common;

namespace PeerAMid.DataAccess;

#nullable enable

public class PeerAMidDataAccess : IPeerAMidDataAccess
{
    private static SessionData SessionData => SessionData.Instance;

    public IDataReader GetAdditionalPeerCompanyList(string industryId, string uid, string companyName,
        decimal revenueStart, decimal revenueTo, int year,
        string regions,
        int iSortCol0, string sSortDir0,
        int optionId, string subIndustryId,
        string? ticker = null)
    {
        Log.Info("optionId = " + optionId);
        // Log.Debug("Entered GetAdditionalPeerCompaniesByRegions");
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
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("[YS].[Proc_GetAdditionalPeerCompaniesByRegions]");
            SetUser(db, dbCmd);
            db.AddInParameter(dbCmd, "@IndustryIds", DbType.String, industryId);
            db.AddInParameter(dbCmd, "@BenchMarkUID", DbType.String, uid);
            db.AddInParameter(dbCmd, "@CompanyName", DbType.String, companyName);
            db.AddInParameter(dbCmd, "@RevenueStart", DbType.Decimal, revenueStart);
            if (revenueTo == 0)
                revenueTo = 1000000000M;
            db.AddInParameter(dbCmd, "@RevenueTo", DbType.Decimal, revenueTo);
            // db.AddInParameter(dbCmd, "@Year", DbType.Int32, year);
            db.AddInParameter(dbCmd, "@PageNumber", DbType.Int32, 1);
            db.AddInParameter(dbCmd, "@PageSize", DbType.Int32, 10);
            db.AddInParameter(dbCmd, "@ColumnId", DbType.Int32, iSortCol0 == 0 ? 4 : iSortCol0);
            db.AddInParameter(
                dbCmd,
                "@Sorting",
                DbType.String,
                string.IsNullOrEmpty(sSortDir0) ? "ASC" : sSortDir0);
            db.AddInParameter(dbCmd, "@OptionId", DbType.Int32, optionId);
            db.AddInParameter(dbCmd, "@Regions", DbType.String, regions ?? "");
            db.AddInParameter(dbCmd, "@SubIndustryIds", DbType.String, subIndustryId);
            if (string.IsNullOrWhiteSpace(ticker))
                db.AddInParameter(dbCmd, "@Ticker", DbType.String, ticker);
            dbCmd.CommandTimeout = 30;
            if (DatabaseAccess.LogDatabaseAccess)
                Log.Info(dbCmd);
            var iDataReader = db.ExecuteReader(dbCmd);
            return iDataReader;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return new DeadDataReader();
        }
    }


    public IDataReader GetSuggestedPeerCompanyList(
        int uid, decimal revenueFrom, decimal revenueTo, int year,
        string? regions,
        string? industryFilter, string? subIndustryFilter,
        string? nameFilter, string? tickerFilter,
        int maxCompanies)
    {
        // Log.Debug("Entered GetAdditionalPeerCompaniesByRegions");
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
            if (revenueTo == 0)
                revenueTo = 1000000000M;

            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("[YS].[Proc_GetSuggestedPeerCompanies]");
            SetUser(db, dbCmd);
            db.AddInParameter(dbCmd, "@BenchmarkCompanyId", DbType.Int32, uid);
            db.AddInParameter(dbCmd, "@RevenueFrom", DbType.Single, revenueFrom);
            db.AddInParameter(dbCmd, "@RevenueTo", DbType.Single, revenueTo);
            db.AddInParameter(dbCmd, "@Year", DbType.Int32, year);
            db.AddInParameter(dbCmd, "@Regions", DbType.String, regions);
            db.AddInParameter(dbCmd, "@IndustryFilter", DbType.String, industryFilter ?? "");
            db.AddInParameter(dbCmd, "@SubIndustryFilter", DbType.String, subIndustryFilter ?? "*");
            db.AddInParameter(dbCmd, "@CompanyNameFilter", DbType.String, nameFilter ?? "");
            db.AddInParameter(dbCmd, "@TickerFilter", DbType.String, tickerFilter ?? "");
            db.AddInParameter(dbCmd, "@MaxCompanies", DbType.Int32, maxCompanies);
            if (DatabaseAccess.LogDatabaseAccess)
                Log.Info(dbCmd);
            var iDataReader = db.ExecuteReader(dbCmd);
            return iDataReader;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return new DeadDataReader();
        }
    }


    public IDataReader GetAutoSuggestCompanyList(string companyId, int optionId, string year, string regions)
    {
        Log.Info("optionId = " + optionId);
        try
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("[YS].[Proc_GetAutoSuggestedCompaniesListByRegions]");
            SetUser(db, dbCmd);
            db.AddInParameter(dbCmd, "@Uid", DbType.String, companyId);
            db.AddInParameter(dbCmd, "@Year", DbType.Int32, year);
            db.AddInParameter(dbCmd, "@OptionId", DbType.Int32, optionId);
            db.AddInParameter(dbCmd, "@Regions", DbType.String, regions ?? "");
            if (DatabaseAccess.LogDatabaseAccess)
                Log.Info(dbCmd);
            var iDataReader = db.ExecuteReader(dbCmd);
            return iDataReader;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return new DeadDataReader();
        }
    }

    public IDataReader GetCompanyDetails(string companyId, string? year)
    {
        try
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("[YS].[Proc_GetCompanyDetail]");
            SetUser(db, dbCmd);
            db.AddInParameter(dbCmd, "@UID", DbType.String, companyId);
            db.AddInParameter(dbCmd, "@Year", DbType.String, year);
            // db.AddInParameter(dbCmd, "@OptionId", DbType.Int32, OptionId);
            if (DatabaseAccess.LogDatabaseAccess)
                Log.Info(dbCmd);
            var iDataReader = db.ExecuteReader(dbCmd);
            return iDataReader;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return new DeadDataReader();
        }
    }

    public IDataReader GetCompanyDetailsByName(string companyName, string? year)
    {
        try
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("[YS].[Proc_GetCompanyDetailByName]");
            SetUser(db, dbCmd);
            db.AddInParameter(dbCmd, "@CompanyName", DbType.String, companyName);
            db.AddInParameter(dbCmd, "@Year", DbType.String, year);
            if (DatabaseAccess.LogDatabaseAccess)
                Log.Info(dbCmd);
            var iDataReader = db.ExecuteReader(dbCmd);
            return iDataReader;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return new DeadDataReader();
        }
    }

    public IDataReader GetCompanyDetailsByTicker(string ticker, string? year)
    {
        try
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("[YS].[Proc_GetCompanyDetailByTicker]");
            SetUser(db, dbCmd);
            db.AddInParameter(dbCmd, "@Ticker", DbType.String, ticker);
            db.AddInParameter(dbCmd, "@Year", DbType.String, year);
            if (DatabaseAccess.LogDatabaseAccess)
                Log.Info(dbCmd);
            var iDataReader = db.ExecuteReader(dbCmd);
            return iDataReader;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return new DeadDataReader();
        }
    }


    public IDataReader GetBenchmarkCompanyList(string cSearch, string tSearch,
        string? regions, string year)
    {
        try
        {
            var db = DbFactory.CreateDatabase();
            // Log.Debug("db = " + db.ConnectionStringWithoutCredentials);
            using var dbCmd = db.GetStoredProcCommand("[YS].[Proc_GetAllCompanyListByRegion]");
            SetUser(db, dbCmd);
            db.AddInParameter(dbCmd, "@Company", DbType.String, cSearch);
            db.AddInParameter(dbCmd, "@Ticker", DbType.String, tSearch);
            db.AddInParameter(dbCmd, "@PageNumber", DbType.Int32, 1);
            db.AddInParameter(dbCmd, "@PageSize", DbType.Int32, 100);
            db.AddInParameter(
                dbCmd,
                "@Regions",
                DbType.String,
                string.IsNullOrEmpty(regions) ? "1,2,3,4,5" : regions);
            if (DatabaseAccess.LogDatabaseAccess)
                Log.Info(dbCmd);
            var reader = db.ExecuteReader(dbCmd);
            return reader;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return new DeadDataReader();
        }
    }

    public IDataReader GetFinalPeersCompanyList(string possiblePeer, string selfPeer, string year, int optionId, string regions)
    {
        Log.Info("optionId = " + optionId);
        try
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("[YS].[Proc_GetFinalListCompaniesByRegions]");
            SetUser(db, dbCmd);
            db.AddInParameter(dbCmd, "@SuggestedCompanyIds", DbType.String, possiblePeer);
            db.AddInParameter(dbCmd, "@SelfSelected", DbType.String, selfPeer);
            //  db.AddInParameter(dbCmd, "@Year", DbType.String, year);
            db.AddInParameter(dbCmd, "@OptionId", DbType.Int32, optionId);
            db.AddInParameter(dbCmd, "@Regions", DbType.String, regions ?? "");
            if (DatabaseAccess.LogDatabaseAccess)
                Log.Info(dbCmd);
            var iDataReader = db.ExecuteReader(dbCmd);
            return iDataReader;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return new DeadDataReader();
        }
    }

    public IDataReader GetPreviouslySelectedPeers(string companyId)
    {
        try
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("[YS].[Proc_GetPreviouslySelectedPeers]");
            SetUser(db, dbCmd);
            db.AddInParameter(dbCmd, "@companyId", DbType.String, companyId);
            if (DatabaseAccess.LogDatabaseAccess)
                Log.Info(dbCmd);
            var iDataReader = db.ExecuteReader(dbCmd);
            return iDataReader;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return new DeadDataReader();
        }
    }

    public IDataReader GetCompanyRequiredData(string companyId)
    {
        try
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("[YS].[Proc_GetCompanyRequireData]");
            SetUser(db, dbCmd);
            db.AddInParameter(dbCmd, "@UID", DbType.String, companyId);
            if (DatabaseAccess.LogDatabaseAccess)
                Log.Info(dbCmd);
            var iDataReader = db.ExecuteReader(dbCmd);
            return iDataReader;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return new DeadDataReader();
        }
    }

    public IDataReader GetCompanyRequiredDataHistory(string companyId)
    {
        try
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("YS.Proc_CompanyDataHistory");
            SetUser(db, dbCmd);
            db.AddInParameter(dbCmd, "@UID", DbType.String, companyId);
            var iDataReader = db.ExecuteReader(dbCmd);
            if (DatabaseAccess.LogDatabaseAccess)
                Log.Info(dbCmd);
            return iDataReader;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return new DeadDataReader();
        }
    }

    public IDataReader GetClientFunctionalData(string companyId, string year)
    {
        try
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("[YS].[Proc_GetClientFunctionalData]");
            SetUser(db, dbCmd);
            db.AddInParameter(dbCmd, "@UID", DbType.String, companyId);
            db.AddInParameter(dbCmd, "@DATAYR", DbType.String, year);
            var iDataReader = db.ExecuteReader(dbCmd);
            if (DatabaseAccess.LogDatabaseAccess)
                Log.Info(dbCmd);
            return iDataReader;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return new DeadDataReader();
        }
    }

    public int SaveRunAnalysis(string filename, string details)
    {
        try
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("[YS].[Proc_SaveRunAnalysisLog]");
            SetUser(db, dbCmd);
            db.AddInParameter(dbCmd, "@FileName", DbType.String, filename);
            db.AddInParameter(dbCmd, "@Details", DbType.String, details);
            if (DatabaseAccess.LogDatabaseAccess)
                Log.Info(dbCmd);
            return db.ExecuteNonQuery(dbCmd);
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return 0;
        }
    }

    public DataTable GetMinMaxRevenueOfCurrentYear()
    {
        DataSet? ds = null;
        try
        {
            var db = DbFactory.CreateDatabase();
            using (var dbCmd = db.GetStoredProcCommand("YS.Proc_getMinMaxRevenueOfCurrentYear"))
            {
                SetUser(db, dbCmd);
                if (DatabaseAccess.LogDatabaseAccess)
                    Log.Info(dbCmd);
                ds = db.ExecuteDataSet(dbCmd);
            }

            return ds.Tables[0];
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return new DataTable();
        }
    }

    public IDataReader GetCurrentFinancialYear()
    {
        try
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("[YS].[Proc_GetCurrentFinancialYear]");
            SetUser(db, dbCmd);
            if (DatabaseAccess.LogDatabaseAccess)
                Log.Info(dbCmd);
            return db.ExecuteReader(dbCmd);
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return new DeadDataReader();
        }
    }

    public int UpdateCurrentFinancialYear(int settingId, int year)
    {
        try
        {
            var db = DbFactory.CreateDatabase();
            var query = "UPDATE ys.ApplicationSetting set CurrentFiscalYear = " +
                        year +
                        "where SettingId = " +
                        settingId;
            using var dbCmdWrapper = db.GetSqlStringCommand(query);
            SetUser(db, dbCmdWrapper);
            if (DatabaseAccess.LogDatabaseAccess)
                Log.Info(dbCmdWrapper);
            var records = db.ExecuteNonQuery(dbCmdWrapper);

            return records;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return 0;
        }
    }

    private static void SetUser(Database db, DbCommand cmd)
    {
        db.AddInParameter(cmd, "@UserId", DbType.String, SessionData.User.Id);
        db.AddInParameter(cmd, "@IsAdmin", DbType.Int32, SessionData.User.IsAdmin ? 1 : 0);
    }
}
