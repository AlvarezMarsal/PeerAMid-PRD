using System.Web.Mvc;
using System.Web.Routing;

namespace YardStickPortal;

public class RouteConfig
{
    public static void RegisterRoutes(RouteCollection routes)
    {
        routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        routes.MapRoute(
            "Default",
            "{controller}/{action}/{id}",
            new
            {
                controller = "Account",
                action = "AboutUs",
                id = UrlParameter.Optional
            }
            //defaults: new { controller = "PeerAMid", action = "Index", id = UrlParameter.Optional }
        );
    }
}