#nullable enable

namespace PeerAMid.Data;

public class SGAPerformanceModel
{
    public List<SGAPerformaceItemModel> SGAPerformanceData { get; set; } = new List<SGAPerformaceItemModel>();
}

public class SGAPerformaceItemModel
{
    private string? _displayName;
    public int PeerCompanyId { get; set; }
    public bool IsTarget { get; set; }
    public decimal XValue { get; set; }
    public decimal YValue { get; set; }

    public decimal X2Value { get; set; }
    public decimal Y2Value { get; set; }
    public string? PeerCompanyName { get; set; }
    public decimal PeerCompanyValue { get; set; }

    public string? PeerCompanyDisplayName
    {
        get => _displayName ?? PeerCompanyName;
        set => _displayName = value;
    }

    public string? ColorCode { get; set; }
    public decimal EBITDAMarginValue { get; set; }
    public decimal SGAMarginValue { get; set; }
}
