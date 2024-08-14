using PeerAMid.Utility;
using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;

#nullable enable

namespace YardStickPortal;

public class MvcApplication : HttpApplication
{
    private static GlobalStaticData? _globalStaticData;
    public static GlobalStaticData GlobalStaticData { get => _globalStaticData!; private set => _globalStaticData = value; }
    public static bool LogRequests { get; set; }

    protected void Application_Start()
    {
        Log.LogFolder = ConfigurationManager.AppSettings.GetForThisMachineNoLog("LogFolder") ?? Path.GetTempPath();
        Log.Info("Starting on machine " + Environment.MachineName);

        _globalStaticData = (GlobalStaticData)Application["GlobalStaticData"];
        if (_globalStaticData == null)
        {
            Log.Info("Loading GSD");
            GlobalStaticData = new GlobalStaticData();
            GlobalStaticData.Load();
            Application["GlobalStaticData"] = GlobalStaticData;

            LogRequests = ConfigurationManager.AppSettings.GetForThisMachine("LogRequests", false);
            CustomAuthorization.LogAuthorizations = ConfigurationManager.AppSettings.GetForThisMachine("LogAuthorizations", false);
            PeerAMid.DataAccess.DatabaseAccess.LogDatabaseAccess = ConfigurationManager.AppSettings.GetForThisMachine("LogDatabaseAccess", false);
        }
        else
        {
            Log.Info("Sharing GSD");
        }

        AreaRegistration.RegisterAllAreas();
        GlobalConfiguration.Configure(WebApiConfig.Register);
        FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        RouteConfig.RegisterRoutes(RouteTable.Routes);
        BundleConfig.RegisterBundles(BundleTable.Bundles);
        UnityConfig.RegisterComponents();

        var logBackgroundWorker = ConfigurationManager.AppSettings.GetForThisMachine("LogBackgroundWorker", false);
        BackgroundWorker.Start(logBackgroundWorker);

    }

    protected void Application_End()
    {
        BackgroundWorker.End();
    }

    protected void Application_BeginRequest(object sender, EventArgs e)
    {
        if (LogRequests)
            Log.Debug("Request: " + Request.Url.ToString());
        Response.AddHeader("Cache-Control", "max-age=0,no-cache,no-store,must-revalidate");
        Response.AddHeader("Pragma", "no-cache");
        Response.AddHeader("Expires", "Tue, 01 Jan 1970 00:00:00 GMT");
    }

    protected void Application_PostAuthorizeRequest()
    {
        if (LogRequests)
            Log.Debug("Request Authorized: " + Request.Url.ToString());
        HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.ReadOnly);
    }

    protected void Session_Start()
    {
        Session.Timeout = 720;
    }
}
