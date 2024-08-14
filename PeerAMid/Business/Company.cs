using Newtonsoft.Json;
// ReSharper disable InconsistentNaming

namespace PeerAMid.Business;

#nullable enable

public class Company
{
    protected readonly CurrencySettings _dataEntrySettings = new();
    protected CurrencySettings _currentSettings = CurrencySettings.Database;

    protected string? _key; // only used by CompanyData

    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public string? CompanyId
    {
        get => Id;
        set => Id = value;
    }

    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public string? CompanyName
    {
        get => Name;
        set => Name = value;
    }

    public string? CompanyNameMixedCase { get; set; }
    public string? CompanyType { get; set; }
    public string? Country { get; set; }
    public int DataYear { get; set; }

    public string DataEntryCurrency
    {
        get => _dataEntrySettings.Currency;
        set => _dataEntrySettings.Currency = value;
    }

    public double DataEntryExchangeRate
    {
        get => _dataEntrySettings.ExchangeRate;
        set => _dataEntrySettings.ExchangeRate = value;
    }

    public string DataEntryUnitOfMeasure
    {
        get => _dataEntrySettings.UnitOfMeasure;
        set => _dataEntrySettings.UnitOfMeasure = value;
    }

    public decimal Ebitda { get; set; }

    private decimal? _ebitdaMargin;
    public decimal EbitdaMargin
    {
        get => _ebitdaMargin ??= (Revenue == 0) ? 0 : Ebitda / Revenue;
        set => _ebitdaMargin = value;
    }

    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public decimal EBITDA
    {
        get => Ebitda;
        set => Ebitda = value;
    }

    public decimal Expense { get; set; }
    public int FinancialYear { get; set; }
    public decimal GrossMargin { get; set; }
    public bool HasWorkingCapitalData { get; set; } = true;
    public int IndustryId { get; set; }
    public bool IsSelected { get; set; } // for use by UI, not backend

    public bool IsSuggested { get; set; }

    public string? Id { get; set; }
    public string? Name { get; set; }
    public string ReportingCurrency => _dataEntrySettings.Currency;
    public decimal Revenue { get; set; }

    public decimal SGA
    {
        get => Expense;
        set => Expense = value;
    }

    public string? ShortName { get; set; }
    public string? ShortNameMixedCase { get; set; }

    public string SicCode
    {
        get => SubIndustryId.ToString();
        set => SubIndustryId = int.Parse(value);
    }

    public int SubIndustryId { get; set; }

    public string? Ticker { get; set; }

    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public string Industry
    {
        set
        {
            // Log.Debug("Industry = " + value);
            if (!string.IsNullOrWhiteSpace(value))
            {
                var space = value.IndexOf(' ');
                if (space > 0)
                    IndustryId = int.Parse(value.Substring(0, space));
            }
        }
    }

    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public string SubIndustry
    {
        set
        {
            // Log.Debug("SubIndustry = " + value);
            if (!string.IsNullOrWhiteSpace(value))
            {
                var space = value.IndexOf(' ');
                if (space > 0)
                    SubIndustryId = int.Parse(value.Substring(0, space));
            }
        }
    }

    public decimal TotalEmployees { get; set; }

    public int TotalRecords { get; set; }

    public int Year
    {
        get => FinancialYear;
        set => FinancialYear = value;
    }

    public string? Yscid { get; set; }

    public string? YSCID
    {
        get => Yscid;
        set => Yscid = value;
    }

    public string? DisplayName
    {
        get
        {
            if (Name == null)
                return null;
            if (!string.IsNullOrEmpty(ShortNameMixedCase))
                return ShortNameMixedCase;
            // Log.Info("No SNMC for " + Name);
            // Log.Info(new System.Diagnostics.StackTrace().ToString());
            if (!string.IsNullOrEmpty(CompanyNameMixedCase))
                return CompanyNameMixedCase;
            // Log.Info("No CompanyNameMixedCase for " + Name);
            return Name;
        }
        set => ShortNameMixedCase = value;
    }

    public bool IsActualData { get; set; }

    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public CurrencySettings CurrentCurrencySettings
    {
        get => _currentSettings;
        set => _currentSettings = value.Clone();
    }

    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public CurrencySettings DataEntryCurrencySettings => _dataEntrySettings;

    public override string ToString()
    {
        return CompanyId + ":" + (CompanyName ?? "");
    }

    public void StupidCopyFrom(Company other, bool everything = false)
    {
        if (other == null)
            return;

        if (everything) _key = other._key;

        CompanyNameMixedCase = other.CompanyNameMixedCase;
        CompanyType = other.CompanyType;
        Country = other.Country;
        //this.Currency = other.Currency;
        DataYear = other.DataYear;
        DataEntryCurrency = other.DataEntryCurrency;
        DataEntryExchangeRate = other.DataEntryExchangeRate;
        Ebitda = other.Ebitda;
        EbitdaMargin = other.EbitdaMargin;
        Expense = other.Expense;
        FinancialYear = other.FinancialYear;
        GrossMargin = other.GrossMargin;
        IndustryId = other.IndustryId;
        if (everything) IsSelected = other.IsSelected;
        if (everything) IsSuggested = other.IsSuggested;
        Id = other.Id;
        Name = other.Name;
        Revenue = other.Revenue;
        ShortName = other.ShortName;
        ShortNameMixedCase = other.ShortNameMixedCase;
        SubIndustryId = other.SubIndustryId;
        Ticker = other.Ticker;
        TotalEmployees = other.TotalEmployees;
        TotalRecords = other.TotalRecords;
        DataEntryUnitOfMeasure = other.DataEntryUnitOfMeasure;
        Yscid = other.Yscid;
        IsActualData = other.IsActualData;
    }

    public void CopyFrom(Company other)
    {
        if (other == null)
            return;
        if (string.IsNullOrEmpty(Id))
            Id = other.Id;
        else if (other.Id != Id)
            Log.Error("Company mismatch!");

        // Log.Debug("this: " + JsonConvert.SerializeObject(this));
        // Log.Debug("other: " + JsonConvert.SerializeObject(other));

        CompanyNameMixedCase = other.CompanyNameMixedCase;
        CompanyType = other.CompanyType;
        Country = other.Country;
        //this.Currency = other.Currency;
        DataYear = other.DataYear;
        Ebitda = other.Ebitda;
        EbitdaMargin = other.EbitdaMargin;
        Expense = other.Expense;
        FinancialYear = other.FinancialYear;
        GrossMargin = other.GrossMargin;
        IndustryId = other.IndustryId;
        IsSelected = other.IsSelected;
        IsSuggested = other.IsSuggested;
        Name = other.Name;
        Revenue = other.Revenue;
        ShortName = other.ShortName;
        ShortNameMixedCase = other.ShortNameMixedCase;
        SubIndustryId = other.SubIndustryId;
        Ticker = other.Ticker;
        TotalEmployees = other.TotalEmployees;
        DataEntryCurrency = other.DataEntryCurrency;
        DataEntryUnitOfMeasure = other.DataEntryUnitOfMeasure;
        DataEntryExchangeRate = other.DataEntryExchangeRate;
        IsActualData = other.IsActualData;
        // Log.Debug("final: " + JsonConvert.SerializeObject(this));
    }

    public void CloneInto(Company clone)
    {
        clone.CopyFrom(this);
    }
}
