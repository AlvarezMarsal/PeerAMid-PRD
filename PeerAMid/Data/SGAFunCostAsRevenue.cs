#nullable enable

namespace PeerAMid.Data;

public class FunctionalRevenueModel
{
    public List<FunctionalRevenueItemModel> FinanceData { get; set; } = [];
    public List<FunctionalRevenueItemModel> SalesData { get; set; } = [];
    public List<FunctionalRevenueItemModel> HRData { get; set; } = [];
    public List<FunctionalRevenueItemModel> MarketData { get; set; } = [];
    public List<FunctionalRevenueItemModel> ITData { get; set; } = [];
    public List<FunctionalRevenueItemModel> CustServData { get; set; } = [];
    public List<FunctionalRevenueItemModel> ProcurementData { get; set; } = [];
    public List<FunctionalRevenueItemModel> CSSupportServData { get; set; } = [];

    public string? FTESummaryLine { get; set; }
}

public class FunctionalRevenueItemModel
{
    private string? _displayName;
    public string? PeerCompanyName { get; set; }
    public decimal PeerCompanyValue { get; set; }
    public string? Color { get; set; }
    public bool IsTarget { get; set; }
    public int RankID { get; set; }

    public string? PeerCompanyDisplayName
    {
        get => _displayName ?? PeerCompanyName;
        set => _displayName = value;
    }
}
