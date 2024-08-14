using PeerAMid.Business;

// ReSharper disable CheckNamespace
#nullable enable

namespace PeerAMid.Core;

public interface IActualDataCollectionCore
{
    // UserCompanyDetails GetUserCompanyDetails();

    int SaveActualDataCollection(ActualDataCollectionModel model);

    int SaveActualDataCollectionEdit(ActualDataCollectionModel model, int uid);

    ActualDataCollectionModel? GetEditNewCompanyPageData(int uid, int year);

    int SaveActualClientDataCollection(ActualDataCollectionModel model);

    int SaveActualLatestDataCollection(ActualDataCollectionModel model);

    ActualDataCollectionModel? GetFilledDataForYear(string companyUID, string yearId);

    ActualDataCollectionModel GetFilledDataForYearEditNew(string companyUID, string yearId);
}
