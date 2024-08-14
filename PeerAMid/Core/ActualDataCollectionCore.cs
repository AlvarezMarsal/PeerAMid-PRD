using PeerAMid.Business;
using PeerAMid.DataAccess;
using System.Data;

#nullable enable

namespace PeerAMid.Core;

public class ActualDataCollectionCore : IActualDataCollectionCore
{
    private readonly IActualDataCollectionDataAccess _iActualDataColllectionDataAccess;

    public ActualDataCollectionCore(IActualDataCollectionDataAccess iActualDataColllectionDataAccess)
    {
        _iActualDataColllectionDataAccess = iActualDataColllectionDataAccess;
    }

    public ActualDataCollectionModel? GetFilledDataForYear(string companyUID, string yearId)
    {
        var model = new ActualDataCollectionModel();
        using var reader = _iActualDataColllectionDataAccess.GetFilledDataForYear(companyUID, yearId);
        if (reader.Read())
        {
            try
            {
                model = ParseCompanyData(reader, model);
                return model;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        return null;
    }

    public ActualDataCollectionModel GetFilledDataForYearEditNew(string companyUID, string yearId)
    {
        var model = new ActualDataCollectionModel();
        // var list = _iActualDataColllectionDataAccess.GetCompanyRequiredData(companyUID);
        using (var reader = _iActualDataColllectionDataAccess.GetCompanyRequiredData(companyUID))
        {
            if (reader.Read()) ParseCompanyData(reader, model);
        }

        using (var reader = _iActualDataColllectionDataAccess.GetFilledDataForYearNew(companyUID, yearId))
        {
            if (reader.Read())
            {
                try
                {
                    ParseCompanyData(reader, model);
                    return model;
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
        }

        return model;
    }

    public ActualDataCollectionModel? GetEditNewCompanyPageData(int uid, int year)
    {
        try
        {
            var model = new ActualDataCollectionModel();
            using var reader = _iActualDataColllectionDataAccess.GetEditNewCompanyPageData(uid, year);
            while (reader?.Read() ?? false)
            {
                try
                {
                    ParseCompanyData(reader, model);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    return null;
                }
            }

            return model;
            //_iActualDataColllectionDataAccess.GetEditNewCompanyPageData(model, uid);
        }
        catch (Exception /*ex*/)
        {
            // Log.Info("ActualDataCollectionCore:GetEditNewCompanyPageData:Error while calling procedure to save/update data", ex.StackTrace);
            return null;
        }
    }

    /*
    public UserCompanyDetails GetUserCompanyDetails()
    {
        var model = new UserCompanyDetails();

        using var ireader = _iActualDataColllectionDataAccess.GetUserCompanyDetails();
        while (ireader.Read())
        {
            try
            {
                model.CompanyID = DBDataHelper.GetInt(ireader, "UID");
                model.CompanyName = DBDataHelper.GetString(ireader, "CompanyName");
                model.YSID = DBDataHelper.GetInt(ireader, "YSID");
                model.Ticker = DBDataHelper.GetString(ireader, "Ticker");
                model.DisplayText = model.CompanyName;
            }
            catch (Exception ex)
            {
                Log.Error(ex);

                return null;
            }
        }

        return model;
    }
    */

    public int SaveActualDataCollection(ActualDataCollectionModel model)
    {
        try
        {
            var dataEntrySettings = model.DataEntryCurrencySettings.Clone();
            model = CostFieldsCalculation.ConvertCurrencyFields(model, CurrencySettings.Database)!; // this sets the data entry settings to the database settings
            model.DataEntryCurrency = dataEntrySettings.Currency;
            model.DataEntryUnitOfMeasure = dataEntrySettings.UnitOfMeasure;
            model.DataEntryExchangeRate = dataEntrySettings.ExchangeRate;

            return _iActualDataColllectionDataAccess.SaveActualDataCollection(model);
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
            var dataEntrySettings = model.DataEntryCurrencySettings.Clone();
            model = CostFieldsCalculation.ConvertCurrencyFields(model, CurrencySettings.Database)!; // this sets the data entry settings to the database settings
            model.DataEntryCurrency = dataEntrySettings.Currency;
            model.DataEntryUnitOfMeasure = dataEntrySettings.UnitOfMeasure;
            model.DataEntryExchangeRate = dataEntrySettings.ExchangeRate;

            return _iActualDataColllectionDataAccess.SaveActualDataCollectionEdit(model, uid);
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
            var dataEntrySettings = model.DataEntryCurrencySettings.Clone();
            model = CostFieldsCalculation.ConvertCurrencyFields(model, CurrencySettings.Database)!; // this sets the data entry settings to the database settings
            model.DataEntryCurrency = dataEntrySettings.Currency;
            model.DataEntryUnitOfMeasure = dataEntrySettings.UnitOfMeasure;
            model.DataEntryExchangeRate = dataEntrySettings.ExchangeRate;

            return _iActualDataColllectionDataAccess.SaveActualClientDataCollection(model);
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
            var dataEntrySettings = model.DataEntryCurrencySettings.Clone();
            model = CostFieldsCalculation.ConvertCurrencyFields(model, CurrencySettings.Database)!; // this sets the data entry settings to the database settings
            model.DataEntryCurrency = dataEntrySettings.Currency;
            model.DataEntryUnitOfMeasure = dataEntrySettings.UnitOfMeasure;
            model.DataEntryExchangeRate = dataEntrySettings.ExchangeRate;

            return _iActualDataColllectionDataAccess.SaveActualLatestDataCollection(model);
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return 0;
        }
    }

    private static ActualDataCollectionModel ParseCompanyData(IDataReader reader, ActualDataCollectionModel? model = null)
    {
        model = model ?? new ActualDataCollectionModel();
        PeerAMidCore.ParseCompany(reader, model);
        return model;
    }
}
