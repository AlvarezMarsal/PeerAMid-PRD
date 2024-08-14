using PeerAMid.Business;
using PeerAMid.Core;
using PeerAMid.Utility;
using System.Web.Http;

#nullable enable

namespace YardStickPortal;

//[AuthorizationPrivilegeFilter]
public class ActualDataCollectionController : ApiController
{
    private readonly IActualDataCollectionCore _iActualDataCollectionCore;

    public ActualDataCollectionController(IActualDataCollectionCore iActualDataCollectionCore)
    {
        _iActualDataCollectionCore = iActualDataCollectionCore;
    }

    /*
    [HttpGet]
    public UserCompanyDetails GetUserCompanyDetails()
    {
        return _IActualDataCollectionCore.GetUserCompanyDetails();
    }
    */

    [HttpPost]
    public object SaveActualDataCollection([FromBody] ActualDataCollectionModel model)
    {
        //Converting cost field values to Millions before saving
        var result = _iActualDataCollectionCore.SaveActualDataCollection(model);
        return new
        {
            result,
            model.YearId
        };
    }

    [HttpPost]
    public object SaveActualDataCollectionEdit([FromBody] ActualDataCollectionModel model)
    {
        var result = _iActualDataCollectionCore.SaveActualDataCollectionEdit(model, model.CompanyUID);
        //Session["UID"] = result;
        return new
        {
            result,
            model.YearId,
            model.CompanyUID
        };
    }

    /// <summary>
    ///     After click on save button SaveActualClientDataCollection function will call for save data in database. Also
    ///     request redirect on PeerSelectionAutoOrSelf page via PeerSelectionAutoOrSelf function.
    ///     Proc_SaveUpdateFunctionalData.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [CalledFromExternalCode]
    public int SaveActualClientDataCollection([FromBody] ActualDataCollectionModel model)
    {
        //Converting cost field values to Millions before saving
        var result = _iActualDataCollectionCore.SaveActualClientDataCollection(model);
        return result;
    }

    [HttpPost]
    public int SaveActualLatestDataCollection([FromBody] ActualDataCollectionModel model)
    {
        var result =
            _iActualDataCollectionCore.SaveActualLatestDataCollection(model);
        return result;
    }

    [HttpGet]
    [CalledFromExternalCode]
    public ActualDataCollectionModel? GetFilledDataForYear(string companyUid, string yearId)
    {
        var model = _iActualDataCollectionCore.GetFilledDataForYear(companyUid, yearId);
        if (model == null)
            return null;
        return CostFieldsCalculation.ConvertCurrencyFields(model, model.DataEntryCurrencySettings)!;
    }

    [HttpGet]
    public ActualDataCollectionModel GetFilledDataForYearEditNew(string companyUid, string yearId)
    {
        var model = _iActualDataCollectionCore.GetFilledDataForYearEditNew(companyUid, yearId);
        return CostFieldsCalculation.ConvertCurrencyFields(model, model.DataEntryCurrencySettings)!;
    }

    [HttpGet]
    public ActualDataCollectionModel GetEditNewCompanyPageData(int uid, int year)
    {
        var model = _iActualDataCollectionCore.GetEditNewCompanyPageData(uid, year);
        return model!;
    }
}
