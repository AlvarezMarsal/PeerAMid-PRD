#nullable enable

namespace PeerAMid.Data;

public class UserCompanyDetails
{
    public string? CompanyName { get; set; }
    public int CompanyID { get; set; }
    public int YSID { get; set; }
    public int DataYear { get; set; }
    public string? Ticker { get; set; }
    public string? DisplayText { get; set; }
}

public class UserCompanyDetailsList
{
    public UserCompanyDetailsList() : this("")
    {
    }

    public UserCompanyDetailsList(string tag)
    {
        Tag = tag;
        Companies = [];
    }

    public List<UserCompanyDetails> Companies { get; set; }
    public string Tag { get; set; }
}
