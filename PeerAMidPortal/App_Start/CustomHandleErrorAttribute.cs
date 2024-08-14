using System;
using System.Web;
using System.Web.Mvc;

namespace YardStickPortal;

public class CustomHandleErrorAttribute : HandleErrorAttribute
{
    public override void OnException(ExceptionContext filterContext)
    {
        if (filterContext.ExceptionHandled)
            return;

        var controllerName = (string) filterContext.RouteData.Values["controller"];
        var actionName = (string) filterContext.RouteData.Values["action"];
        Log.Error("ASP Exception in " + controllerName + "." + actionName, filterContext.Exception);

        if (!filterContext.HttpContext.IsCustomErrorEnabled) 
            return;

        if (new HttpException(null, filterContext.Exception).GetHttpCode() != 500) return;
        if (!ExceptionType.IsInstanceOfType(filterContext.Exception)) return;
        if (filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            filterContext.Result = new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new
                {
                    error = true,
                    message = filterContext.Exception.Message
                }
            };
        }
        else
        {
            var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);
            filterContext.Result = new ViewResult
            {
                ViewName = "Error",
                MasterName = Master,
                ViewData = new ViewDataDictionary(model),
                TempData = filterContext.Controller.TempData
            };
            System.Log.Error($"[{controllerName}/{actionName}]", filterContext.Exception);
        }

        filterContext.ExceptionHandled = true;
        filterContext.HttpContext.Response.Clear();
        filterContext.HttpContext.Response.StatusCode = 500;
        filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
    }
}
