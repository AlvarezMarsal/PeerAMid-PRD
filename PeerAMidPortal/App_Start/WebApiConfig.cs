using System.Web.Http;
using System.Web.Routing;

namespace YardStickPortal;

public static class WebApiConfig
{
    public static void Register(HttpConfiguration config)
    {
        // Web API routes
        config.MapHttpAttributeRoutes();

        RouteTable.Routes.MapHttpRoute(
            "DefaultApi",
            "api/{controller}/{action}/{id}",
            new
            {
                id = RouteParameter.Optional
            }
        );
    }
}