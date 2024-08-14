using PeerAMid.Support;
using System;
using System.Web.Mvc;
using System.Web.Security;

namespace YardStickPortal;

public class CustomAuthorization : ActionFilterAttribute
{
    public bool AdminOnly { get; set; }
    public static bool LogAuthorizations = false;

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        var allow = SessionData.Instance.User.IsAuthenticated;
        if (allow && AdminOnly)
            allow = SessionData.Instance.User.IsAdmin;

        if (allow)
        {
            if (LogAuthorizations)
            {
                Log.Debug("Call into " + filterContext.ActionDescriptor.ActionName + "(" + string.Join(",", filterContext.ActionParameters) + ") was allowed");
            }
            return;
        }

        SessionData.Instance.User.Id = null;
        FormsAuthentication.SignOut();
        if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
        {
            filterContext.Result = new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new
                {
                    error = true,
                    messageType = "Logout",
                    message = "Session Expired"
                }
            };
            filterContext.RequestContext.HttpContext.Response.StatusCode = 401;
            filterContext.RequestContext.HttpContext.Response.StatusDescription = "Session Expired";
            filterContext.RequestContext.HttpContext.Response.End();
            if (LogAuthorizations)
            {
                Log.Debug("Ajax call into " + filterContext.ActionDescriptor.ActionName + "(" + string.Join(",", filterContext.ActionParameters) + ") was disallowed");
            }
        }
        else
        {
            //filterContext.Result = new RedirectResult(string.Format("/Account/NotAuthorize"));
            filterContext.Result = new RedirectResult("/");
            if (LogAuthorizations)
            {
                Log.Debug("Call into " + filterContext.ActionDescriptor.ActionName + "(" + string.Join(",", filterContext.ActionParameters) + ") was disallowed");
            }
        }
    }
}
