using Newtonsoft.Json;
using PeerAMid.Business;
using PeerAMid.Utility;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Web;

#nullable enable

namespace PeerAMid.Support;

public class SessionData
{
    private static bool? _workingCapitalIsAvailable;
    private bool? _resetAnalysisOnBenchmarkCompanySelectionPage;

    public readonly UserData User = new();

    private readonly List<Company> _peerSearchResults = [];

    private readonly List<CompanyData> _selectedPeerCompanies = [];

    static SessionData()
    {
        var assembly = System.Reflection.Assembly.GetExecutingAssembly();
        var version = FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
        BuildNumber = version;
    }

    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public static SessionData Instance
    {
        get
        {
            var instance = HttpContext.Current.Session["SessionData"];
            if (instance != null)
                return (SessionData) instance;

            var sd = new SessionData();
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var version = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
            SessionData.BuildNumber = version;
            HttpContext.Current.Session["SessionData"] = sd;
            return sd;
        }
    }

    public CompanyData BenchmarkCompany { get; } = new("Benchmark");


    public static bool WorkingCapitalIsAvailable
    {
        get
        {
            _workingCapitalIsAvailable ??= GetFromConfig("WorkingCapitalIsAvailable", true);
            return _workingCapitalIsAvailable.Value;
        }
    }

    public bool ResetAnalysisOnBenchmarkCompanySelectionPage
    {
        get
        {
            _resetAnalysisOnBenchmarkCompanySelectionPage ??= GetFromConfig("ResetAnalysisOnBenchmarkCompanySelectionPage", true);
            return _resetAnalysisOnBenchmarkCompanySelectionPage.Value;
        }
    }

    public bool ReloadForModifyPeers { get; set; } = false;

    public static int AutoLogOffTimeout // in seconds
    {
        get
        {
            var saveToSession = false;
            var value = GetFromSession<int>("AutoLogOffTimeout", 1800);
            if (value == 0)
            {
                saveToSession = true;
                var s = GetFromConfig("AutoLogOffTimeout", "1800"); // thirty minutes
                if (!int.TryParse(s, out value))
                    value = 1800;
            }

            var v = MathHelper.Bounded(value, 30 * 60, 24 * 60 * 60);
            if (saveToSession || v != value)
                SetInSession("AutoLogOffTimeout", value = v);
            return value;
        }

        set
        {
            var v = MathHelper.Bounded(value, 30 * 60, 24 * 60 * 60);
            SetInSession("AutoLogOffTimeout", v);
        }
    }

    private static bool? _singleSignOnEnabled;
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public static bool SingleSignOnEnabled
    {
        get => _singleSignOnEnabled ??= GetFromConfig("SingleSignOnEnabled", true);
        set => _singleSignOnEnabled = value;
    }

    public static object? GlobalStaticData { get; set; }

    public PeerAMidService SelectedService { get; set; } = PeerAMidService.None;
    private int _optionId;
    public int OptionId
    {
        get { /*Log.Info("Getting optionid = " + _optionId);*/ return _optionId; }
        set { /*Log.Info("Setting optionid = " + value);*/ _optionId = value; }
    }

    // Benchmark Company Search parameters
    // The settings on the benchmark company search page
    // public bool HasBenchmarkCompanySearchParameters { get; set; }

    public string SelectedRegions { get; set; } = "1,2,3,4,5";  // also applies to other pages

#pragma warning disable IDE1006 // Naming Styles
    public string cSearch { get; set; } = "";
    public string tSearch { get; set; } = "";
#pragma warning restore IDE1006 // Naming Styles

    public enum PeerSelectionModes : int
    {
        NoneChosen,
        AutomaticPick,
        SelfPick,
        ManuallyEnterData
    }

    public PeerSelectionModes PeerSelectionMode { get; set; }

    public List<Company> BenchmarkCompanySearchResults { get; set; } = [];  // TODO -- remove

    // Peer Selection parameters
    // the settings on the peer selection page
    // public bool HasPeerSelectionParameters { get; set; }

    //public long PeerSelectionMinRevenue { get; set; } // in even dollars
    //public long PeerSelectionMaxRevenue { get; set; } = -1; // in even dollars; -1 means no upper limit
    //public string PeerSelectionCompanySearchPrefix { get; set; } = "";
    //public string PeerSelectionTickerSearchPrefix { get; set; } = "";
    //public string PeerSelectionSelectedIndustries { get; set; } = "";
    //public string PeerSelectionSelectedSubIndustries { get; set; } = "";



    public int PeerSelOption { get; set; }
    public bool IsPossiblePeerCompanies { get; set; }
    public bool IsSelfPeerCompanies { get; set; }
    public string PossiblePeerCompanies { get; set; } = string.Empty;
    public string SelfPeerCompanies { get; set; } = string.Empty;
    public string AdditionalPeerCompanies { get; set; } = string.Empty;
    public string FinalPeerCompanies { get; set; } = string.Empty;
    public int BackFromRunAnalysis { get; set; }
    public int CurrentFinancialYear { get; set; }
    public object? ForcedResultFromCompanyAutoSelection { get; set; } // = null; // a hack
    public string? PreviousPptFileName { get; set; } = string.Empty;
    public string? SelectedIndustries { get; set; }
    public string? SelectedSubIndustries { get; set; }
    public static string BuildNumber { get; set; } = "1.0.0.0";
    public static string BuildConfiguration { get; set; } = ""; // this is set by DbFactory and really indicates which DB we are using

    public List<CompanyData> SelectedPeerCompanies { get => _selectedPeerCompanies; set { _selectedPeerCompanies.Clear(); _selectedPeerCompanies.AddRange(value); } }
    public List<Company> PeerSearchResults { get => _peerSearchResults; set { _peerSearchResults.Clear(); _peerSearchResults.AddRange(value); } }


    [CalledFromExternalCode]
    public static string SelectedCurrency { get; set; } = "USD";

    //[JsonIgnore]
    //[System.Text.Json.Serialization.JsonIgnore]
    //public PageStateStacks PageStateStacks { get; } = new();

    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public static int LifespanOfPrivateDataHours
    {
        get
        {
            var lopdh = GetFromConfig("LifespanOfPrivateDataHours", "72"); // enough to last from Friday to Monday
            return string.IsNullOrWhiteSpace(lopdh) || !int.TryParse(lopdh, out var s) ? 72 : s;
        }
    }

    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public static int BackupPrivateData
    {
        get
        {
            var bupd = GetFromConfig("BackupPrivateData", "1");
            return string.IsNullOrWhiteSpace(bupd) || !int.TryParse(bupd, out var s) ? 1 : s;
        }
    }

    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public static int HoursBetweenPurges
    {
        get
        {
            var hbp = GetFromConfig("HoursBetweenPurges", "6");
            return string.IsNullOrWhiteSpace(hbp) || !int.TryParse(hbp, out var s) ? 6 : s;
        }
    }

    private static bool? _impersonateComServerExec;
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public static bool ImpersonateComServerExec
    {
        get
        {
            _impersonateComServerExec ??= GetFromConfig("ImpersonateComServerExec", true);
            return _impersonateComServerExec.Value;
        }
    }


    private static string? _templatePath;
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public static string TemplatePath
    {
        get
        {
            if (_templatePath == null)
            {
                _templatePath = GetFromConfig("TemplatePath", "");
                if (string.IsNullOrWhiteSpace(_templatePath))
                {
                    // var asm = System.Reflection.Assembly.GetExecutingAssembly();
                    var users = ConfigurationManager.AppSettings.GetForThisMachine("UsersFolder", @"C:\www\PeerAMidPortal\App_Data\Users")!;
                    var appData = Path.GetDirectoryName(users);
                    var pp = Path.GetDirectoryName(appData);
                    _templatePath = Path.Combine(pp, "PPTTemplate");
                }
                Log.Info("Template path: " + _templatePath);
            }
            return _templatePath;
        }
    }


    //[JsonIgnore]
    //[System.Text.Json.Serialization.JsonIgnore]
    //public bool ExcelIsVisible => GetFromConfig("ExcelIsVisible", true);


    public override string ToString()
    {
        return JavaScriptConverter.ToJavaScript(this);
    }

    // [return: NotNull]
    private static T GetFromSession<T>(string key, T defaultValue)
    {
        try
        {
            var obj = HttpContext.Current.Session[key];
            if (obj == null) return defaultValue;
            return (T)obj;
        }
        catch
        {
            return defaultValue;
        }
    }

    /*
    private static T? GetFromSession<T>(string key)
    {
        try
        {
            var obj = HttpContext.Current.Session[key];
            if (obj == null) return default(T);
            return (T)obj;
        }
        catch
        {
            return default;
        }
    }
    */

    private static void SetInSession<T>(string key, T value)
    {
        if (value == null)
            HttpContext.Current.Session.Remove(key);
        else
            HttpContext.Current.Session[key] = value;
    }

    #region from Web.Config

    private static string GetFromConfig(string name, string defaultValue)
    {
        var v = ConfigurationManager.AppSettings.Get(name + "-" + Environment.MachineName) ??
                ConfigurationManager.AppSettings.Get(name);

        if (v == null)
            return defaultValue;

        return v;
    }

    private static bool GetFromConfig(string name, bool defaultValue)
    {
        var v = GetFromConfig(name, defaultValue ? "true" : "false");
        return bool.TryParse(v, out var result) ? result : defaultValue;
    }

    #endregion from Web.Config

    #region User

    public class UserData
    {
        private string? _folder;

        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public string? Id { get; set; }

        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public string? Name { get; set; }

        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public string? UserName { get; set; } // Like ALVAREZMARSAL/Skhalivikar

        public string? Email { get; set; }

        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public bool IsAuthenticated { get; set; }

        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public bool IsAdmin { get; set; }

        //public IReadOnlyCollection<string> Roles => _roles;
        public int LogId { get; set; }

        public bool IsSgaDiagnosticUser => AllowFullSGA;
        public bool IsWorkingCapitalUser => AllowFullWCD;

        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public bool AllowFullSGA { get; set; }


        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public bool AllowFullWCD { get; set; }

        public bool IsRetailUser { get; set; }

        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public string? Folder
        {
            get
            {
                if (_folder == null)
                {
                    _folder = ConfigurationManager.AppSettings.GetForThisMachine("UsersFolder", @"C:\www\PeerAMidPortal\App_Data\Users")!;
                    if (!_folder.EndsWith("\\"))
                        _folder += "\\";
                    _folder += Name;
                    if (!Directory.Exists(_folder))
                    {
                        try
                        {
                            Directory.CreateDirectory(_folder);
                        }
                        catch (Exception ex)
                        {
                            _folder = null;
                            Log.Error(ex);
                        }
                    }
                }

                return _folder;
            }
        }
    }

    #endregion User

    #region Company

    public class CompanyData : Company
    {
        public CompanyData()
        {
            _key = Guid.NewGuid().ToString();
        }

        public CompanyData(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                _key = Guid.NewGuid().ToString();
            else
                _key = key;
        }

        public CompanyData(Company c, bool copyEverything = false)
        {
            StupidCopyFrom(c, copyEverything);
        }

        public int Uid
        {
            get => int.TryParse(Id ?? "X", out var uid) ? uid : 0;
            set => Id = value.ToString();
        }
    }

    #endregion Company
}
