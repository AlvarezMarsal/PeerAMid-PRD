namespace PeerAMid.Business;

#nullable enable

public class CostFieldsCalculation
{
    public static ActualDataCollectionModel? ConvertCurrencyFields(ActualDataCollectionModel? model, CurrencySettings settings)
    {
        if (model == null)
            return null;

        if (model.CurrentCurrencySettings.Equals(settings))
            return model;

        model.SGACostCorporateSupportServices = settings.ConvertFrom(model.SGACostCorporateSupportServices, model.CurrentCurrencySettings);
        model.SGACostCustomerServices = settings.ConvertFrom(model.SGACostCustomerServices, model.CurrentCurrencySettings);
        model.SGACostFinance = settings.ConvertFrom(model.SGACostFinance, model.CurrentCurrencySettings);
        model.SGACostHumanResources = settings.ConvertFrom(model.SGACostHumanResources, model.CurrentCurrencySettings);
        model.SGACostIT = settings.ConvertFrom(model.SGACostIT, model.CurrentCurrencySettings);
        model.SGACostMarketing = settings.ConvertFrom(model.SGACostMarketing, model.CurrentCurrencySettings);
        model.SGACostProcurement = settings.ConvertFrom(model.SGACostProcurement, model.CurrentCurrencySettings);
        model.SGACostSales = settings.ConvertFrom(model.SGACostSales, model.CurrentCurrencySettings);
        model.Revenue = settings.ConvertFrom(model.Revenue, model.CurrentCurrencySettings);
        model.EBITDA = settings.ConvertFrom(model.EBITDA, model.CurrentCurrencySettings);
        model.SGA = settings.ConvertFrom(model.SGA, model.CurrentCurrencySettings);
        model.GrossMargin = settings.ConvertFrom(model.GrossMargin, model.CurrentCurrencySettings);
        model.CurrentCurrencySettings = settings;

        return model;
    }
}
