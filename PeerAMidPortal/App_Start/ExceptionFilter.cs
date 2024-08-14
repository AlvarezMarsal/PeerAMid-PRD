using System.Diagnostics;
using System.Web.Mvc;

namespace YardStickPortal;

public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext filterContext)
    {
        Trace.TraceError(filterContext.Exception.ToString());
    }
}