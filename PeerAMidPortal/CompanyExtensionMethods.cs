using PeerAMid.Business;

#nullable enable

namespace YardStickPortal;

public static class CompanyExtensionMethods
{
    // IndustryId + " " + IndustryName
    public static string? GetIndustry(this Company company)
    {
        return MvcApplication.GlobalStaticData.GetIndustry(company.IndustryId);
    }

    public static string? GetIndustryName(this Company company)
    {
        return MvcApplication.GlobalStaticData.GetIndustryName(company.IndustryId);
    }

    public static string? GetSubIndustry(this Company company)
    {
        return MvcApplication.GlobalStaticData.GetSubIndustry(company.SubIndustryId);
    }

    public static string? GetSubIndustryName(this Company company)
    {
        return MvcApplication.GlobalStaticData.GetSubIndustryName(company.SubIndustryId);
    }
}
