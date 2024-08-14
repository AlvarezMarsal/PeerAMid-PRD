using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace YardStickPortal;

public class NoAuthorizationAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(HttpActionContext filterContext)
    {
    }
}
