// ReSharper disable InconsistentNaming
namespace PeerAMid.Business;

#nullable enable

public class ActualDataCollectionModel : Company
{
    public ActualDataCollectionModel()
    {
    }

    public int Result { get; set; }

    public int CompanyUID
    {
        get => int.Parse(Id);
        set => Id = value.ToString();
    }

    public string? CompanyID
    {
        get => Id;
        set => Id = value;
    }

    public string UserId { get; set; } = "";

    public int YearId
    {
        get => DataYear;
        set => DataYear = value;
    }

    public int? TotalNumOfEmployee
    {
        get => (int)TotalEmployees;
        set => TotalEmployees = value.GetValueOrDefault(0);
    }

    public double? SGACostFinance { get; set; }
    public double? SGACostHumanResources { get; set; }
    public double? SGACostIT { get; set; }
    public double? SGACostProcurement { get; set; }
    public double? SGACostCorporateSupportServices { get; set; }
    public double? SGACostCustomerServices { get; set; }
    public double? SGACostSales { get; set; }
    public double? SGACostMarketing { get; set; }

    public double? FTEFinance { get; set; }
    public double? FTEHumanResources { get; set; }
    public double? FTEIT { get; set; }
    public double? FTEProcurement { get; set; }
    public double? FTECorporateSupportServices { get; set; }
    public double? FTECustomerServices { get; set; }
    public double? FTESales { get; set; }
    public double? FTEMarketing { get; set; }

    public int IsOptionalData { get; set; }

    public ActualDataCollectionModel Clone()
    {
        var clone = new ActualDataCollectionModel();
        CloneInto(clone);
        return clone;
    }

    public void CloneInto(ActualDataCollectionModel clone)
    {
        base.CloneInto(clone);
        clone.Result = Result;
        clone.UserId = UserId;
        clone.SGACostFinance = SGACostFinance;
        clone.SGACostHumanResources = SGACostHumanResources;
        clone.SGACostIT = SGACostIT;
        clone.SGACostProcurement = SGACostProcurement;
        clone.SGACostCorporateSupportServices = SGACostCorporateSupportServices;
        clone.SGACostCustomerServices = SGACostCustomerServices;
        clone.SGACostSales = SGACostSales;
        clone.SGACostMarketing = SGACostMarketing;
        clone.FTEFinance = FTEFinance;
        clone.FTEHumanResources = FTEHumanResources;
        clone.FTEIT = FTEIT;
        clone.FTEProcurement = FTEProcurement;
        clone.FTECorporateSupportServices = FTECorporateSupportServices;
        clone.FTECustomerServices = FTECustomerServices;
        clone.FTESales = FTESales;
        clone.FTEMarketing = FTEMarketing;
        clone.IsOptionalData = IsOptionalData;
    }
}
