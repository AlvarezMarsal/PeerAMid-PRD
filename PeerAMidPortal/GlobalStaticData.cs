using Microsoft.Practices.EnterpriseLibrary.Data;
using PeerAMid.Business;
using PeerAMid.Data;
using PeerAMid.DataAccess;
using PeerAMid.Utility;
using System;
using System.Collections.Generic;

#nullable enable

namespace YardStickPortal;

public class GlobalStaticData
{
    private string? _asJavaScript;
    private List<Currency> _currencies = new();
    private DateTime _currencyLoadTime = DateTime.MinValue;
    private string _selectedCurrency = "USD";
    public List<Region> AllRegions { get; } = new();
    public List<Country> AllCountries { get; } = new();
    //private readonly SortedList<int, ExtendedIndustryModel> _industries = new();
    public List<ExtendedIndustryModel> Industries { get; } = new();

    public string SelectedCurrency
    {
        get => _selectedCurrency ?? "USD";
        set => _selectedCurrency = value ?? "USD";
    }

    //public List<CompanyFastSearchByName> CompanyFastSearch { get; } = new();
    public int TotalCompanies { get; private set; }

    public string CurrentVersion { get; set; } = "";

    public List<Currency> Currencies
    {
        get
        {
            if (_currencyLoadTime < DateTime.Now.AddMinutes(-10))
            {
                var db = DbFactory.CreateDatabase();
                LoadCurrencies(db);
            }

            return _currencies;
        }

        set
        {
            _currencies = value;
            _currencyLoadTime = DateTime.Now;
        }
    }

    public override string ToString()
    {
        return _asJavaScript ??= JavaScriptConverter.ToJavaScript(this);
    }

    public void Load()
    {
        //HttpContext.Current.Session["GlobalStaticData"] = gsd;
        try
        {
            CurrentVersion = DateTime.Now.Ticks.ToString();

            var db = DbFactory.CreateDatabase();
            LoadCurrencies(db);

            using (var dbCmd = db.GetStoredProcCommand("[YS].[Proc_GetRegions]"))
            {
                if (DatabaseAccess.LogDatabaseAccess)
                    Log.Info(dbCmd);
                var reader = db.ExecuteReader(dbCmd);
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var noc = DBDataHelper.GetInt(reader, "NumberOfCompanies");
                        TotalCompanies += noc;

                        AllRegions.Add(new Region(DBDataHelper.GetInt(reader, "UID"), DBDataHelper.GetString(reader, "Name"), /*DBDataHelper.GetString(reader, "Currency"),*/ noc));
                    }
                }
            }

            using (var dbCmd = db.GetStoredProcCommand("[YS].[Proc_GetCountries]"))
            {
                if (DatabaseAccess.LogDatabaseAccess)
                    Log.Info(dbCmd);
                var reader = db.ExecuteReader(dbCmd);
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        AllCountries.Add(new Country(DBDataHelper.GetString(reader, "COUNTRY"),
                                                     DBDataHelper.GetString(reader, "CURRENCY"),
                                                     DBDataHelper.GetInt(reader, "REGIONID")));
                    }
                }
            }

            using (var dbCmd = db.GetStoredProcCommand("[YS].[Proc_GetExtendedIndustryList]"))
            {
                if (DatabaseAccess.LogDatabaseAccess)
                    Log.Info(dbCmd);
                var reader = db.ExecuteReader(dbCmd);
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var industryId = DBDataHelper.GetInt(reader, "IndustryId");
                        var industryName = DBDataHelper.GetString(reader, "IndustryName");
                        var industry = new ExtendedIndustryModel(industryId, industryName, true);
                        Industries.Add(industry);
                    }

                    Industries.Sort((a, b) => a.IndustryId.CompareTo(b.IndustryId));

                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            var iid = DBDataHelper.GetInt(reader, "IndustryId");
                            var sid = DBDataHelper.GetInt(reader, "SubIndustryId");
                            var parent = Industries.Find(i => i.IndustryId == iid);
                            if (parent != null)
                            {
                                var subIndustryName = DBDataHelper.GetString(reader, "SubIndustryName");
                                parent.SubIndustries.Add(new SubIndustryModel(iid, sid, subIndustryName));
                            }
                            else
                            {
                                Log.Error($"No industry found matching '{iid}'");
                            }
                        }
                    }
                }

                //_asJavaScript = JavaScriptConverter.ToJavaScript(this);
                // Log.Debug(_asJavaScript);
                //HttpContext.Current.Session["GlobalStaticData"] = gsd;
            }

            _asJavaScript = JavaScriptConverter.ToJavaScript(this);
            // Log.Debug(_asJavaScript);
            //HttpContext.Current.Session["GlobalStaticData"] = gsd;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
    }

    private ExtendedIndustryModel? FindIndustry(string industryId)
    {
        return FindIndustry(int.Parse(industryId));
    }

    private ExtendedIndustryModel? FindIndustry(int industryId)
    {
        return Industries.Find(x => x.IndustryId == industryId);
    }


    public string? GetIndustryName(string industryId)
    {
        var i = FindIndustry(industryId);
        if (i != null)
            // Log.Debug("Industry: " + industryId + ", II=" + i.IndustryId + ", IN=" + i.IndustryName);
            return i.IndustryName;
        return null;
    }

    public string? GetIndustryName(int industryId)
    {
        var i = FindIndustry(industryId);
        if (i != null)
        {
            // Log.Debug("Industry: " + industryId + ", II=" + i.IndustryId + ", IN=" + i.IndustryName);
            return i.IndustryName;
        }
        return null;
    }

    public string? GetIndustry(string industryId)
    {
        return GetIndustryName(industryId);
    }

    public string? GetIndustry(int industryId)
    {
        return GetIndustryName(industryId);
    }

    private SubIndustryModel? FindSubIndustry(string subIndustryId)
    {
        var sid = int.Parse(subIndustryId);
        return FindSubIndustry(sid);
    }

    private SubIndustryModel? FindSubIndustry(int subIndustryId)
    {
        var industryId = subIndustryId / 100;
        var i = FindIndustry(industryId);
        if (i == null)
            return null;
        return i.SubIndustries.Find(x => x.SubIndustryId == subIndustryId);
    }

    public string GetSubIndustryName(string subIndustryId)
    {
        // Log.Debug("Looking up SUBIND " + subIndustryId ?? "null");
        var s = FindSubIndustry(subIndustryId);
        if (s != null)
        {
            // Log.Debug("SubIndustry: " + subIndustryId + ", SI=" + s.SubIndustryId + ", SN=" + s.SubIndustryName);
            // Log.Debug("Found " + s.SubIndustryName ?? "null");
            return s.SubIndustryName;
        }
        // Log.Debug("Not found");
        return "Unknown subindustry";
    }

    public string GetSubIndustryName(int subIndustryId)
    {
        // Log.Debug("Looking up SUBIND " + subIndustryId ?? "null");
        var s = FindSubIndustry(subIndustryId);
        if (s != null)
        {
            // Log.Debug("SubIndustry: " + subIndustryId + ", SI=" + s.SubIndustryId + ", SN=" + s.SubIndustryName);
            // Log.Debug("Found " + s.SubIndustryName ?? "null");
            return s.SubIndustryName;
        }
        // Log.Debug("Not found");
        return "Unknown subindustry";
    }


    public string GetSubIndustry(string subIndustryId)
    {
        return GetSubIndustryName(subIndustryId);
    }

    public string GetSubIndustry(int subIndustryId)
    {
        return GetSubIndustryName(subIndustryId);
    }

    private void LoadCurrencies(Database db)
    {
        try
        {
            var currencies = new Dictionary<string, Currency>();
            using (var dbCmd = db.GetStoredProcCommand("[YS].[Proc_GetCurrencies]"))
            {
                if (DatabaseAccess.LogDatabaseAccess)
                    Log.Info(dbCmd);
                var reader = db.ExecuteReader(dbCmd);
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var name = DBDataHelper.GetString(reader, "CURRENCY");
                        var year = DBDataHelper.GetNullableInt(reader, "YEAR");
                        var exchangeRate = DBDataHelper.GetNullableDouble(reader, "EXCHANGERATE");

                        if (!currencies.TryGetValue(name, out var currency))
                        {
                            currencies.Add(name, currency = new Currency
                            {
                                Name = name,
                                Description = name == "USD" ? "United States Dollar" : name,
                                SmallValueLabel = name,
                                SmallValueFormat = name == "USD" ? "$*" : "*",
                                LargeValueLabel = name + "MM",
                                LargeValueFormat = name == "USD" ? "$*MM" : "*MM",
                                AmericanFormatting = name == "USD" ? 1 : 0,
                                CanBeUsedForReports = 1 // 1 for USD, EUR, GBP, 0 for others
                            });
                        }

                        if (year.HasValue)
                            currency.ExchangeRateByYear.Add(new YearlyExchangeRate { Year = year.Value, ExchangeRate = exchangeRate.GetValueOrDefault(1) });
                    }
                }
            }

            _currencies = new List<Currency>(currencies.Values);
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }

        _currencyLoadTime = DateTime.Now;
    }

    public double GetExchangeRate(string currency, int year)
    {
        for (var i = 0; i < Currencies.Count; i++)
        {
            if (Currencies[i].Name == currency)
                return Currencies[i].GetExchangeRate(year);
        }

        return 1;
    }
}
