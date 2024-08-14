using PeerAMid.Business;
using System.Data;

namespace PeerAMid.DataAccess;

public interface IActualDataCollectionDataAccess
{
    // IDataReader GetUserCompanyDetails();

    int SaveActualDataCollection(ActualDataCollectionModel model);

    int SaveActualDataCollectionEdit(ActualDataCollectionModel model, int uid);

    IDataReader GetEditNewCompanyPageData(int uid, int year);

    int SaveActualClientDataCollection(ActualDataCollectionModel model);

    int SaveActualLatestDataCollection(ActualDataCollectionModel model);

    IDataReader GetFilledDataForYear(string companyUID, string yearId);

    IDataReader GetFilledDataForYearNew(string companyUID, string yearId);

    IDataReader GetCompanyRequiredData(string companyId);
}
