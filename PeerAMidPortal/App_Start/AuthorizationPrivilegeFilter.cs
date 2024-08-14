namespace YardStickPortal;

/*
public class AuthorizationPrivilegeFilter : ActionFilterAttribute
{
    private static readonly object CheckLock = new();

    public override void OnActionExecuting(HttpActionContext filterContext)
    {
        bool fail;
        lock (CheckLock)
        {
            IEnumerable<string> values;
            fail = !(filterContext.Request.Headers.TryGetValues("Authorization", out values) &&
                     values.First() == "vOuzTZH04SDfWy0XQ49wSQ");
        }

        if (fail)
        {
            var response = Response<string>.Create(
                false,
                ResponseMessages.AuthorizationFailed,
                HttpStatusCode.Unauthorized,
                ResponseMessages.DataNotReceived,
                DateTime.Now);
            var responseString = JsonConvert.SerializeObject(response);
            filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.Unauthorized, responseString);
            base.OnActionExecuting(filterContext);
        }
    }
}
*/
