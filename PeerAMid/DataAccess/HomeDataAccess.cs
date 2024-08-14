using Microsoft.Practices.EnterpriseLibrary.Data;
using PeerAMid.Data;
using PeerAMid.Support;
using System.Data;
using System.Data.Common;

#nullable enable

namespace PeerAMid.DataAccess;

public class HomeDataAccess : IHomeDataAccess
{
    /*
    public IDataReader GetIndustryList(string userId)
    {
        try
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("[YS].[Proc_GetIndustryList]");
            SetUser(db, dbCmd);
            db.AddInParameter(dbCmd, "@SearchText", DbType.String, null);
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
    */

    public IDataReader GetSICIndustryCompanyList(string phase, int optionID, int max)
    {
        if (DatabaseAccess.LogDatabaseAccess)
            Log.Info("optionId = " + optionID);
        try
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("[YS].[Proc_GetIndustryAndCompanyList]");
            SetUser(db, dbCmd);
            db.AddInParameter(dbCmd, "@SearchText", DbType.String, phase);
            db.AddInParameter(dbCmd, "@OptionId", DbType.Int32, optionID);
            db.AddInParameter(dbCmd, "@MaxCount", DbType.Int32, max);
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

    public IDataReader GetSubIndustryList(string? industryId, string? subIndustryText = "")
    {
        try
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("[YS].[Proc_GetSubIndustryList]");
            SetUser(db, dbCmd);
            db.AddInParameter(dbCmd, "@IndustryId", DbType.String, industryId);
            db.AddInParameter(dbCmd, "@SubIndustry", DbType.String, subIndustryText ?? "");
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

    public IDataReader GetPeerCompanyList(string industryId, string subIndustoryList)
    {
        try
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("[YS].[Proc_GetPeerCompaniesList]");
            SetUser(db, dbCmd);
            db.AddInParameter(dbCmd, "@SubIndustryIds", DbType.String, subIndustoryList);
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

    public IDataReader GetDemographicChartData(int targetCompanyId, string selectedPeerList, int year, int optionId)
    {
        Log.Info("optionId = " + optionId);
        try
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("[YS].[Proc_ChartOne]");
            SetUser(db, dbCmd);
            db.AddInParameter(dbCmd, "@ListofPeer", DbType.String, selectedPeerList);
            db.AddInParameter(dbCmd, "@TargetCompany", DbType.Int32, targetCompanyId);
            db.AddInParameter(dbCmd, "@Year", DbType.Int32, year);
            db.AddInParameter(dbCmd, "@OptionId", DbType.Int32, optionId);
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

    public IDataReader GetFutureYears()
    {
        try
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("[YS].[Proc_FutureYearList]");
            SetUser(db, dbCmd);
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

    public IDataReader GetSGAChartData(int targetCompanyId, string selectedPeerList, int year, int optionId, bool requireMatchingFiscalYear = false)
    {
        Log.Info("optionId = " + optionId);
        try
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("[YS].[Proc_ChartTwo]");
            SetUser(db, dbCmd);
            db.AddInParameter(dbCmd, "@ListofPeer", DbType.String, selectedPeerList);
            db.AddInParameter(dbCmd, "@TargetCompany", DbType.Int32, targetCompanyId);
            db.AddInParameter(dbCmd, "@Year", DbType.Int32, year);
            db.AddInParameter(dbCmd, "@OptionId", DbType.Int32, optionId);
            db.AddInParameter(dbCmd, "@RequireMatchingFiscalYear", DbType.Boolean, requireMatchingFiscalYear);
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

    public IDataReader GetWorkingCapitalData(int targetCompanyId, string selectedPeerList, int year, int optionId,
        bool requireMatchingFiscalYear = false)
    {
        Log.Info("optionId = " + optionId);
        try
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("[YS].[Proc_GetWorkingCapitalData]");
            SetUser(db, dbCmd);
            db.AddInParameter(dbCmd, "@ListofPeer", DbType.String, selectedPeerList);
            db.AddInParameter(dbCmd, "@TargetCompany", DbType.Int32, targetCompanyId);
            db.AddInParameter(dbCmd, "@Year", DbType.Int32, year);
            db.AddInParameter(dbCmd, "@OptionId", DbType.Int32, optionId);
            db.AddInParameter(dbCmd, "@RequireMatchingFiscalYear", DbType.Boolean, requireMatchingFiscalYear);
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

    public IDataReader GetWorkingCapitalTrendData(int targetCompanyId, string selectedPeerList, int firstYear,
        int lastYear)
    {
        try
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("[YS].[Proc_GetWorkingCapitalTrendData]");
            SetUser(db, dbCmd);
            db.AddInParameter(dbCmd, "@ListofPeer", DbType.String, selectedPeerList);
            db.AddInParameter(dbCmd, "@TargetCompany", DbType.Int32, targetCompanyId);
            db.AddInParameter(dbCmd, "@FirstYear", DbType.Int32, firstYear);
            db.AddInParameter(dbCmd, "@LastYear", DbType.Int32, lastYear);
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

    public IDataReader GetSelectedTargetAndSubIndustriesModel(string subIndustoryList)
    {
        try
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("[YS].[Proc_CompanyIndAndSubIndustry]");
            SetUser(db, dbCmd);
            db.AddInParameter(dbCmd, "@PeersList", DbType.String, subIndustoryList);
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

    public IDataReader GetSGAPerformanceChartData(int targetCompanyId, string selectedPeerList, int year,
        int optionId)
    {
        Log.Info("optionId = " + optionId);
        try
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("[YS].[Proc_ChartThree]");
            SetUser(db, dbCmd);
            db.AddInParameter(dbCmd, "@ListofPeer", DbType.String, selectedPeerList);
            db.AddInParameter(dbCmd, "@TargetCompany", DbType.Int32, targetCompanyId);
            db.AddInParameter(dbCmd, "@Year", DbType.Int32, year);
            db.AddInParameter(dbCmd, "@OptionId", DbType.Int32, optionId);
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

    public IDataReader GetFunCostAsPercentOfRevenueData(int targetCompanyId, string selectedPeerList, int year,
        int optionId)
    {
        Log.Info("optionId = " + optionId);
        try
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("[YS].[Proc_ChartFour]");
            SetUser(db, dbCmd);
            db.AddInParameter(dbCmd, "@ListofPeer", DbType.String, selectedPeerList);
            db.AddInParameter(dbCmd, "@TargetCompany", DbType.Int32, targetCompanyId);
            db.AddInParameter(dbCmd, "@Year", DbType.Int32, year);
            db.AddInParameter(dbCmd, "@OptionId", DbType.Int32, optionId);
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

    public IDataReader GetFTEDecomposedCostAsPercentOfRevenueData(int targetCompanyId, string selectedPeerList,
        int year, int optionId)
    {
        Log.Info("optionId = " + optionId);
        try
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("[YS].[Proc_ChartSix]");
            SetUser(db, dbCmd);
            db.AddInParameter(dbCmd, "@ListofPeer", DbType.String, selectedPeerList);
            db.AddInParameter(dbCmd, "@TargetCompany", DbType.Int32, targetCompanyId);
            db.AddInParameter(dbCmd, "@Year", DbType.Int32, year);
            db.AddInParameter(dbCmd, "@OptionId", DbType.Int32, optionId);
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

    public IDataReader GetFinalPeerAndTargetCompaniesList(int targetCompanyId, string selectedPeerList, int year)
    {
        try
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("[YS].[Proc_GetFinalPeerAndTargetCompaniesList]");
            SetUser(db, dbCmd);
            db.AddInParameter(dbCmd, "@PeerCompanyIds", DbType.String, selectedPeerList);
            db.AddInParameter(dbCmd, "@TargetCOmpany", DbType.String, targetCompanyId);
            db.AddInParameter(dbCmd, "@Year", DbType.Int32, year);
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

    public IDataReader GetDecomposedSGAWaterFallTopQ(int targetCompanyId, string selectedPeerList, int year,
        int optionId)
    {
        Log.Info("optionId = " + optionId);
        try
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("[YS].[Proc_ChartFive]");
            SetUser(db, dbCmd);
            db.AddInParameter(dbCmd, "@ListofPeer", DbType.String, selectedPeerList);
            db.AddInParameter(dbCmd, "@TargetCompany", DbType.Int32, targetCompanyId);
            db.AddInParameter(dbCmd, "@Year", DbType.Int32, year);
            db.AddInParameter(dbCmd, "@OptionId", DbType.Int32, optionId);
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

    public IDataReader GetDecomposedSGAWaterFallMedian(int targetCompanyId, string selectedPeerList, int year,
        int optionId)
    {
        if (DatabaseAccess.LogDatabaseAccess)
            Log.Info("optionId = " + optionId);
        try
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("[YS].[Proc_MedianChartFive]");
            SetUser(db, dbCmd);
            db.AddInParameter(dbCmd, "@ListofPeer", DbType.String, selectedPeerList);
            db.AddInParameter(dbCmd, "@TargetCompany", DbType.Int32, targetCompanyId);
            db.AddInParameter(dbCmd, "@Year", DbType.Int32, year);
            db.AddInParameter(dbCmd, "@OptionId", DbType.Int32, optionId);
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

    public IDataReader GetDecomposedSGAWaterFallTopD(int targetCompanyId, string selectedPeerList, int year,
        int optionId)
    {
        Log.Info("optionId = " + optionId);
        try
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("[YS].[Proc_ChartFiveTopDecile]");
            SetUser(db, dbCmd);
            db.AddInParameter(dbCmd, "@ListofPeer", DbType.String, selectedPeerList);
            db.AddInParameter(dbCmd, "@TargetCompany", DbType.Int32, targetCompanyId);
            db.AddInParameter(dbCmd, "@Year", DbType.Int32, year);
            db.AddInParameter(dbCmd, "@OptionId", DbType.Int32, optionId);
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

    public IDataReader GetAppPhrases(string app, string language, long versionCutoff)
    {
        try
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("[YS].[Proc_GetAppPhrases]");
            SetUser(db, dbCmd);
            db.AddInParameter(dbCmd, "@App", DbType.String, app);
            db.AddInParameter(dbCmd, "@Language", DbType.String, language);
            db.AddInParameter(dbCmd, "@VersionCutoff", DbType.Int32, versionCutoff);
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

    private static void SetUser(Database db, DbCommand cmd)
    {
        db.AddInParameter(cmd, "@UserId", DbType.String, SessionData.Instance.User.Id);
        db.AddInParameter(cmd, "@IsAdmin", DbType.Int32, SessionData.Instance.User.IsAdmin ? 1 : 0);
    }
}
