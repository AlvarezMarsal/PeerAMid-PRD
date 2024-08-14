using Newtonsoft.Json;
using System.Text;

#nullable enable

namespace PeerAMid.Data;

public record class IndustryModel
{
    public int IndustryId { get; }
    public string IndustryName { get; }

    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public bool IsIndustry { get; }

    public IndustryModel(int id, string name, bool isIndustry)
    {
        IndustryId = id;
        IndustryName = name;
        IsIndustry = isIndustry;
    }
}

public record class SubIndustryModel
{
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public int IndustryId { get; }

    public int SubIndustryId { get; }
    public string SubIndustryName { get; }

    public SubIndustryModel(int industryId, int subIndustryId, string name)
    {
        IndustryId = industryId;
        SubIndustryId = subIndustryId;
        SubIndustryName = name;
    }

    public override string ToString()
    {
        return IndustryId + " " + SubIndustryId + " " + SubIndustryName;
    }
}

public record class ExtendedIndustryModel : IndustryModel
{
    public List<SubIndustryModel> SubIndustries { get; } = new();

    public ExtendedIndustryModel(int id, string name, bool isIndustry)
        : base(id, name, isIndustry)
    {
    }
}


public class PeerCompanyModel
{
    private string? _displayName;
    public string? IndustryId { get; set; }
    public string? SubIndustryId { get; set; }
    public int PeerCompanyId { get; set; }
    public string? PeerCompanyName { get; set; }

    public string? PeerCompanyDisplayName
    {
        get => _displayName ?? PeerCompanyName;
        set => _displayName = value;
    }
}

public class SelectedTargetAndSubIndustriesModel
{
    public string? IndustryName { get; set; }
    public List<string> SubIndustryNames { get; set; } = [];
    public string? TargetSymbol { get; set; }

    public override string ToString()
    {
        var b = new StringBuilder();
        AppendToString(b, "");
        return b.ToString();
    }

    public void AppendToString(StringBuilder b, string? indent = null)
    {
        indent = indent ?? "";
        b.Append(indent).Append("IndustryName = " + IndustryName).AppendLine();
        b.Append(indent).Append("SubIndustryNames").AppendLine();
        foreach (var n in SubIndustryNames)
            b.Append(indent).Append(n).AppendLine();
        b.Append(indent).Append("TargetSymbol = " + TargetSymbol);
    }
}
