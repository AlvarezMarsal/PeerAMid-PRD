using PeerAMid.Utility;

namespace PeerAMid.Support;

public enum PeerAMidService
{
    [Description("None")]
    [ReportTitle("None")]
    None = 0,

    [Description("SG&A Full Functional Diagnostics")]
    [ReportTitle("SG&A Full Functional Diagnostics")]
    SgaFull = 1,

    [Description("Working Capital Full Diagnostics")]
    [ReportTitle("Working Capital Full Diagnostics")]
    WcdFull = 2,

    [Description("SG&A Cost High Level Diagnostics")]
    [ReportTitle("SG&A Cost High Level Diagnostics")]
    SgaShort = 3,

    [Description("Cash Conversion Cycle Diagnostics")]
    [ReportTitle("Cash Conversion Cycle Diagnostics")]
    WcdShort = 4,

    [Description("Retail Diagnostic")]
    [ReportTitle("Retail Diagnostic")]
    RetailFull = 5,

    [Description("Retail High Level Cost Diagnostic")]
    [ReportTitle("Retail High Level Cost Diagnostics")]
    RetailShort = 6
}

public static class PeerAMidServiceExtensionMethods
{
    public static bool IsSGA(this PeerAMidService service)
    {
        return service is PeerAMidService.SgaShort or PeerAMidService.SgaFull;
    }

    public static bool IsWCD(this PeerAMidService service)
    {
        return service is PeerAMidService.WcdShort or PeerAMidService.WcdFull;
    }

    public static bool IsShortForm(this PeerAMidService service)
    {
        return service is PeerAMidService.WcdShort or PeerAMidService.SgaShort;
    }

    public static bool IsLongForm(this PeerAMidService service)
    {
        return service is PeerAMidService.WcdFull or PeerAMidService.SgaFull;
    }
}