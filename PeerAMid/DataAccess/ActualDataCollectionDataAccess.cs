using Microsoft.Practices.EnterpriseLibrary.Data;
using PeerAMid.Business;
using PeerAMid.Data;
using PeerAMid.Support;
using System.Data;
using System.Data.Common;

namespace PeerAMid.DataAccess;

public class ActualDataCollectionDataAccess : IActualDataCollectionDataAccess
{
    private static SessionData SessionData => SessionData.Instance;

    public IDataReader GetFilledDataForYear(string companyUID, string yearId)
    {
        try
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("YS.Proc_GetClientActualData");
            SetUser(db, dbCmd);
            db.AddInParameter(dbCmd, "@UID", DbType.String, companyUID);
            db.AddInParameter(dbCmd, "@DATAYR", DbType.String, yearId);
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

    public IDataReader GetFilledDataForYearNew(string companyUID, string yearId)
    {
        try
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("YS.Proc_GetClientActualData");
            SetUser(db, dbCmd);
            db.AddInParameter(dbCmd, "@UID", DbType.String, companyUID);
            db.AddInParameter(dbCmd, "@DATAYR", DbType.String, yearId);
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
            //db.AddInParameter(dbCmd, "@UserId", DbType.String, userid);
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

    public IDataReader GetEditNewCompanyPageData(int uid, int year)
    {
        try
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("[YS].[Proc_GetCompanyInformationwithData]");
            SetUser(db, dbCmd);
            db.AddInParameter(dbCmd, "@Uid", DbType.Int32, uid);
            db.AddInParameter(dbCmd, "@DataYear", DbType.Int32, year);
            if (DatabaseAccess.LogDatabaseAccess)
                Log.Info(dbCmd);
            var iDataReader = db.ExecuteReader(dbCmd);
            return iDataReader;
        }
        catch (Exception /*ex*/)
        {
            // Log.Info("ActualDataCollectionDataAccess:SaveActualDataCollection:Error while saving/updating data collection details", ex.StackTrace);
            return new DeadDataReader();
        }
    }

    /*
    public IDataReader GetUserCompanyDetails()
    {
        try
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("YS.Proc_UserCompanyList");
            SetUser(db, dbCmd);
            Log.Info(dbCmd);
            iDataReader = db.ExecuteReader(dbCmd);
            return iDataReader;
        }
        catch (Exception ex)
        {
            Log.Error(ex);

            return null;
        }
    }
    */

    public int SaveActualDataCollection(ActualDataCollectionModel model)
    {
        try
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("[YS].[Proc_NewCompanyAndSaveUpdateClientData]");
            SetUser(db, dbCmd);
            db.AddInParameter(dbCmd, "@US1K", DbType.String, model.CompanyType);
            db.AddInParameter(dbCmd, "@coname", DbType.String, model.CompanyName);
            db.AddInParameter(dbCmd, "@DATAYR", DbType.Int32, model.YearId);
            db.AddInParameter(dbCmd, "@SubIndustry", DbType.String, model.SubIndustryId.ToString("0000"));
            db.AddInParameter(dbCmd, "@Ticker", DbType.String, model.Ticker);
            db.AddInParameter(dbCmd, "@REPCURR", DbType.String, model.DataEntryCurrency);
            db.AddInParameter(dbCmd, "@TOTEE", DbType.Int32, model.TotalNumOfEmployee);
            db.AddInParameter(dbCmd, "@REVMNA", DbType.Double, model.Revenue);
            db.AddInParameter(dbCmd, "@EBITDAMN", DbType.Double, model.EBITDA);
            db.AddInParameter(dbCmd, "@SGAMN", DbType.Double, model.SGA);
            db.AddInParameter(dbCmd, "@GrossMN", DbType.Double, model.GrossMargin);
            db.AddInParameter(dbCmd, "@IsOptional", DbType.Boolean, model.IsOptionalData);
            db.AddInParameter(dbCmd, "@FNCOSTMN", DbType.Double, model.SGACostFinance);
            db.AddInParameter(dbCmd, "@HRCOSTMN", DbType.Double, model.SGACostHumanResources);
            db.AddInParameter(dbCmd, "@ITCOSTMN", DbType.Double, model.SGACostIT);
            db.AddInParameter(dbCmd, "@PRCOSTMN", DbType.Double, model.SGACostProcurement);
            db.AddInParameter(dbCmd, "@CORPCOSTMN", DbType.Double, model.SGACostCorporateSupportServices);
            db.AddInParameter(dbCmd, "@SCOSTMN", DbType.Double, model.SGACostSales);
            db.AddInParameter(dbCmd, "@CSCOSTMN", DbType.Double, model.SGACostCustomerServices);
            db.AddInParameter(dbCmd, "@MKCOSTMN", DbType.Double, model.SGACostMarketing);
            db.AddInParameter(dbCmd, "@FNFTEA", DbType.Double, model.FTEFinance);
            db.AddInParameter(dbCmd, "@HRFTEA", DbType.Double, model.FTEHumanResources);
            db.AddInParameter(dbCmd, "@ITFTEA", DbType.Double, model.FTEIT);
            db.AddInParameter(dbCmd, "@PRFTEA", DbType.Double, model.FTEProcurement);
            db.AddInParameter(dbCmd, "@CORPFTEA", DbType.Double, model.FTECorporateSupportServices);
            db.AddInParameter(dbCmd, "@SFTEA", DbType.Double, model.FTESales);
            db.AddInParameter(dbCmd, "@CSFTEA", DbType.Double, model.FTECustomerServices);
            db.AddInParameter(dbCmd, "@MKFTEA", DbType.Double, model.FTEMarketing);
            db.AddInParameter(dbCmd, "@ActionBy", DbType.String, model.UserId);
            db.AddInParameter(dbCmd, "@CostUnit", DbType.String, model.DataEntryUnitOfMeasure);
            db.AddInParameter(dbCmd, "@Country", DbType.String, model.Country ?? "UNITED STATES");
            db.AddInParameter(dbCmd, "@ExRate", DbType.Double, model.DataEntryExchangeRate);
            db.AddInParameter(dbCmd, "@UnitOfMeasure", DbType.String, model.DataEntryUnitOfMeasure.Trim());
            db.AddOutParameter(dbCmd, "@UID", DbType.Int32, 0);
            if (DatabaseAccess.LogDatabaseAccess)
                Log.Info(dbCmd);
            db.ExecuteNonQuery(dbCmd);
            model.CompanyUID = int.Parse(db.GetParameterValue(dbCmd, "@UID").ToString());
            return model.CompanyUID;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return 0;
        }
    }

    public int SaveActualDataCollectionEdit(ActualDataCollectionModel model, int uid)
    {
        try
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("[YS].[Proc_NewCompanyAndSaveUpdateClientDataWithUpdate]");
            SetUser(db, dbCmd);
            db.AddInParameter(dbCmd, "@US1K", DbType.String, model.CompanyType);
            db.AddInParameter(dbCmd, "@coname", DbType.String, model.CompanyName);
            db.AddInParameter(dbCmd, "@DATAYR", DbType.Int32, model.YearId);
            db.AddInParameter(dbCmd, "@SubIndustry", DbType.String, model.SubIndustryId.ToString("0000"));
            db.AddInParameter(dbCmd, "@IndustryId", DbType.String, model.IndustryId.ToString("00"));
            db.AddInParameter(dbCmd, "@Ticker", DbType.String, model.Ticker);
            db.AddInParameter(dbCmd, "@REPCURR", DbType.String, model.DataEntryCurrency);
            db.AddInParameter(dbCmd, "@TOTEE", DbType.Int32, model.TotalNumOfEmployee);
            db.AddInParameter(dbCmd, "@REVMNA", DbType.Double, model.Revenue);
            db.AddInParameter(dbCmd, "@EBITDAMN", DbType.Double, model.EBITDA);
            db.AddInParameter(dbCmd, "@SGAMN", DbType.Double, model.SGA);
            db.AddInParameter(dbCmd, "@GrossMN", DbType.Double, model.GrossMargin);
            //db.AddInParameter(dbCmd, "@IsOptional", DbType.Boolean, model.IsOptionalData);
            db.AddInParameter(dbCmd, "@FNCOSTMN", DbType.Double, model.SGACostFinance);
            db.AddInParameter(dbCmd, "@HRCOSTMN", DbType.Double, model.SGACostHumanResources);
            db.AddInParameter(dbCmd, "@ITCOSTMN", DbType.Double, model.SGACostIT);
            db.AddInParameter(dbCmd, "@PRCOSTMN", DbType.Double, model.SGACostProcurement);
            db.AddInParameter(dbCmd, "@CORPCOSTMN", DbType.Double, model.SGACostCorporateSupportServices);
            db.AddInParameter(dbCmd, "@SCOSTMN", DbType.Double, model.SGACostSales);
            db.AddInParameter(dbCmd, "@CSCOSTMN", DbType.Double, model.SGACostCustomerServices);
            db.AddInParameter(dbCmd, "@MKCOSTMN", DbType.Double, model.SGACostMarketing);
            db.AddInParameter(dbCmd, "@FNFTEA", DbType.Double, model.FTEFinance);
            db.AddInParameter(dbCmd, "@HRFTEA", DbType.Double, model.FTEHumanResources);
            db.AddInParameter(dbCmd, "@ITFTEA", DbType.Double, model.FTEIT);
            db.AddInParameter(dbCmd, "@PRFTEA", DbType.Double, model.FTEProcurement);
            db.AddInParameter(dbCmd, "@CORPFTEA", DbType.Double, model.FTECorporateSupportServices);
            db.AddInParameter(dbCmd, "@SFTEA", DbType.Double, model.FTESales);
            db.AddInParameter(dbCmd, "@CSFTEA", DbType.Double, model.FTECustomerServices);
            db.AddInParameter(dbCmd, "@MKFTEA", DbType.Double, model.FTEMarketing);
            db.AddInParameter(dbCmd, "@ActionBy", DbType.String, model.UserId);
            db.AddInParameter(dbCmd, "@YSID", DbType.Int32, model.YSCID);
            db.AddInParameter(dbCmd, "@UID", DbType.Int32, model.CompanyUID);
            db.AddInParameter(dbCmd, "@CostUnit", DbType.String, model.DataEntryUnitOfMeasure);
            db.AddInParameter(dbCmd, "@Country", DbType.String, model.Country ?? "UNITED STATES");
            db.AddInParameter(dbCmd, "@ExRate", DbType.Double, model.DataEntryExchangeRate);
            db.AddInParameter(dbCmd, "@UnitOfMeasure", DbType.String, model.DataEntryUnitOfMeasure.Trim());
            db.AddOutParameter(dbCmd, "@newUID", DbType.Int32, 0);
            db.ExecuteNonQuery(dbCmd);
            if (DatabaseAccess.LogDatabaseAccess)
                Log.Info(dbCmd);
            return Convert.ToInt32(db.GetParameterValue(dbCmd, "@newUID"));
            //return 1;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return 0;
        }
    }

    public int SaveActualClientDataCollection(ActualDataCollectionModel model)
    {
        try
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("[YS].[Proc_SaveUpdateFunctionalData]");
            SetUser(db, dbCmd);
            db.AddInParameter(dbCmd, "@UID", DbType.Int32, model.CompanyUID);
            //db.AddInParameter(dbCmd, "@YSCID", DbType.Int32, model.YSCID);
            db.AddInParameter(dbCmd, "@DATAYR", DbType.Int32, model.YearId);
            db.AddInParameter(dbCmd, "@REVMNA", DbType.Double, model.Revenue);
            var grossMargin = model.Revenue * model.GrossMargin;
            db.AddInParameter(dbCmd, "@GROSSMN", DbType.Double, grossMargin);

            db.AddInParameter(dbCmd, "@SGAMN", DbType.Double, model.SGA);
            var eBITDA = model.Revenue * model.EBITDA;
            db.AddInParameter(dbCmd, "@EBITDAMN", DbType.Double, eBITDA);
            db.AddInParameter(dbCmd, "@TOTEE", DbType.Int32, model.TotalNumOfEmployee);

            db.AddInParameter(dbCmd, "@FNCOSTMN", DbType.Double, model.SGACostFinance);
            db.AddInParameter(dbCmd, "@HRCOSTMN", DbType.Double, model.SGACostHumanResources);
            db.AddInParameter(dbCmd, "@ITCOSTMN", DbType.Double, model.SGACostIT);
            db.AddInParameter(dbCmd, "@PRCOSTMN", DbType.Double, model.SGACostProcurement);
            db.AddInParameter(dbCmd, "@CORPCOSTMN", DbType.Double, model.SGACostCorporateSupportServices);
            db.AddInParameter(dbCmd, "@SCOSTMN", DbType.Double, model.SGACostSales);
            db.AddInParameter(dbCmd, "@CSCOSTMN", DbType.Double, model.SGACostCustomerServices);
            db.AddInParameter(dbCmd, "@MKCOSTMN", DbType.Double, model.SGACostMarketing);
            db.AddInParameter(dbCmd, "@FNFTEA", DbType.Double, model.FTEFinance);
            db.AddInParameter(dbCmd, "@HRFTEA", DbType.Double, model.FTEHumanResources);
            db.AddInParameter(dbCmd, "@ITFTEA", DbType.Double, model.FTEIT);
            db.AddInParameter(dbCmd, "@PRFTEA", DbType.Double, model.FTEProcurement);
            db.AddInParameter(dbCmd, "@CORPFTEA", DbType.Double, model.FTECorporateSupportServices);
            db.AddInParameter(dbCmd, "@SFTEA", DbType.Double, model.FTESales);
            db.AddInParameter(dbCmd, "@CSFTEA", DbType.Double, model.FTECustomerServices);
            db.AddInParameter(dbCmd, "@MKFTEA", DbType.Double, model.FTEMarketing);
            db.AddInParameter(dbCmd, "@CostUnit", DbType.String, model.DataEntryUnitOfMeasure);
            db.AddInParameter(dbCmd, "@ActionBy", DbType.String, SessionData.User.Id);
            db.AddInParameter(dbCmd, "@ExRate", DbType.Double, model.DataEntryExchangeRate);
            db.AddInParameter(dbCmd, "@UnitOfMeasure", DbType.String, model.DataEntryUnitOfMeasure.Trim());
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

    public int SaveActualLatestDataCollection(ActualDataCollectionModel model)
    {
        try
        {
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("[YS].[Proc_SaveUpdateClientData]");
            SetUser(db, dbCmd);
            var r = (double)(model.Revenue);
            if (r == 0)
                r = 1;
            db.AddInParameter(dbCmd, "@UID", DbType.Int32, model.CompanyUID);
            //db.AddInParameter(dbCmd, "@YSCID", DbType.Int32, model.YSCID);
            db.AddInParameter(dbCmd, "@DATAYR", DbType.Int32, model.YearId);
            db.AddInParameter(dbCmd, "@REPCURR", DbType.String, model.DataEntryCurrency);
            db.AddInParameter(dbCmd, "@TOTEE", DbType.Int32, model.TotalEmployees);
            db.AddInParameter(dbCmd, "@REVMNA", DbType.Double, r);
            db.AddInParameter(dbCmd, "@EBITDAMN", DbType.Double, model.EBITDA);
            db.AddInParameter(dbCmd, "@GrossMN", DbType.Double, model.GrossMargin);
            db.AddInParameter(dbCmd, "@SGAMN", DbType.Double, model.SGA);
            db.AddInParameter(dbCmd, "@IsOptional", DbType.Boolean, true);
            db.AddInParameter(dbCmd, "@FNCOSTMN", DbType.Double, (model.SGACostFinance! / r));
            db.AddInParameter(dbCmd, "@HRCOSTMN", DbType.Double, (model.SGACostHumanResources! / r));
            db.AddInParameter(dbCmd, "@ITCOSTMN", DbType.Double, (model.SGACostIT! / r));
            db.AddInParameter(dbCmd, "@PRCOSTMN", DbType.Double, (model.SGACostProcurement! / r));
            db.AddInParameter(dbCmd, "@CORPCOSTMN", DbType.Double, (model.SGACostCorporateSupportServices! / r));
            db.AddInParameter(dbCmd, "@SCOSTMN", DbType.Double, (model.SGACostSales! / r));
            db.AddInParameter(dbCmd, "@CSCOSTMN", DbType.Double, (model.SGACostCustomerServices! / r));
            db.AddInParameter(dbCmd, "@MKCOSTMN", DbType.Double, (model.SGACostMarketing! / r));
            db.AddInParameter(dbCmd, "@FNFTEA", DbType.Double, model.FTEFinance);
            db.AddInParameter(dbCmd, "@HRFTEA", DbType.Double, model.FTEHumanResources);
            db.AddInParameter(dbCmd, "@ITFTEA", DbType.Double, model.FTEIT);
            db.AddInParameter(dbCmd, "@PRFTEA", DbType.Double, model.FTEProcurement);
            db.AddInParameter(dbCmd, "@CORPFTEA", DbType.Double, model.FTECorporateSupportServices);
            db.AddInParameter(dbCmd, "@SFTEA", DbType.Double, model.FTESales);
            db.AddInParameter(dbCmd, "@CSFTEA", DbType.Double, model.FTECustomerServices);
            db.AddInParameter(dbCmd, "@MKFTEA", DbType.Double, model.FTEMarketing);
            db.AddInParameter(dbCmd, "@CostUnit", DbType.String, model.DataEntryUnitOfMeasure);
            db.AddInParameter(dbCmd, "@ActionBy", DbType.String, SessionData.User.Id);
            db.AddInParameter(dbCmd, "@ExRate", DbType.Double, model.DataEntryExchangeRate);
            db.AddInParameter(dbCmd, "@UnitOfMeasure", DbType.String, model.DataEntryUnitOfMeasure);
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

    private static void SetUser(Database db, DbCommand dbCmd)
    {
        db.AddInParameter(dbCmd, "@UserId", DbType.String, SessionData.User.Id);
        db.AddInParameter(dbCmd, "@IsAdmin", DbType.Int32, SessionData.User.IsAdmin ? 1 : 0);
    }
}
