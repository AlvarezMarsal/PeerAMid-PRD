using PeerAMid.Support;
using System.Web.Mvc;

#nullable enable

namespace YardStickPortal;

/// <summary>
///     Previously use this class for pdf converter (Not in use)
/// </summary>
[AllowAnonymous]
public class HomeController : Controller
{
    private static SessionData SessionData => SessionData.Instance;

    /// <summary>
    ///     Previously use for manage session data in viewbag that is MVC feature (Not in use)
    /// </summary>
    /// <returns></returns>
    [CustomHandleError]
    [CustomAuthorization]
    public ActionResult Index()
    {
        ViewBag.BenchmarkCompany = SessionData.BenchmarkCompany.Id;
        ViewBag.BenchmarkCompanyName = SessionData.BenchmarkCompany.Name;
        ViewBag.BenchmarkCompanyRevenue = SessionData.BenchmarkCompany.Revenue;
        ViewBag.FinalPeerCompanies = SessionData.FinalPeerCompanies ?? "";
        ViewBag.Industry = SessionData.BenchmarkCompany.GetIndustry();
        ViewBag.SubIndustry = SessionData.BenchmarkCompany.GetSubIndustry();
        return View();
    }
}
