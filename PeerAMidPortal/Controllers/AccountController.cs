using PeerAMid.Business;
using PeerAMid.DataAccess;
using PeerAMid.Support;
using PeerAMid.Utility;
using System;
using System.Data;
using System.Web.Mvc;

#nullable enable

namespace YardStickPortal;

/// <summary>
///     Purpose of below class for validate credentials on login time.
/// </summary>
public class AccountController : Controller
{
    private SessionData SessionData => SessionData.Instance;

    /// <summary>
    ///     After click on Login Menu Request redirect on login page.
    /// </summary>
    /// <returns></returns>
    public ActionResult UserLogin()
    {
        // Authenticate user (Member of PeerAMid AD Group)
        // This is controlled by Engineering and we're stuck with it
        if (!User.IsPeerAMidUser())
            // Not an authorized user - redirect to front page
            return Redirect("/Account/AboutUs");

        // Initialize Session Data
        // Log.Info("Logging in");
        var sd = SessionData.Instance;
        sd.User.Id = User.Identity.Name;                // Like "ALVAREZMARSAL\RSummers"
        var login = User.Identity.GetLogin();           // Like "RSummers"
        if (login == null)
        {
            Log.Error("Failed to get login name");
            return Redirect("/Account/AboutUs");
        }
        sd.User.Name = login;
        sd.User.Email = login.ToLower() + "@alvarezandmarsal.com";
        sd.User.IsAuthenticated = true; // he must be in the PeerAMid AD group, and is therefore authenticated
        sd.User.UserName = User.Identity.Name;
        sd.User.LogId = 0;

        try
        {
            var db = DbFactory.CreateDatabase();
            var dbCmd = db.GetStoredProcCommand("YS.Proc_LogIn");
            {
                db.AddInParameter(dbCmd, "@LoginName", DbType.String, sd.User.Id);
                db.AddInParameter(dbCmd, "@Email", DbType.String, SessionData.Instance.User.Email);
                db.AddOutParameter(dbCmd, "@LogId", DbType.Int32, 0);
                if (DatabaseAccess.LogDatabaseAccess)
                    Log.Info(dbCmd);
                db.ExecuteNonQuery(dbCmd);
                sd.User.LogId = Convert.ToInt32(db.GetParameterValue(dbCmd, "@LogId"));
                Log.Info("LogId = " + SessionData.Instance.User.LogId);
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to log in {sd.User.Id} : {sd.User.Name}", ex);
            SessionData.Instance.User.LogId = 0;
        }

        if (sd.User.LogId == 0)
        {
            sd.User.Id = null;
            sd.User.Name = null;
            sd.User.IsAuthenticated = false;
            return Redirect("/Account/AboutUs");
        }

        string? roles = null;
        try
        {
            var db = DbFactory.CreateDatabase();
            var dbCmd = db.GetStoredProcCommand("YS.Proc_GetUserRoles");
            {
                db.AddInParameter(dbCmd, "@UserName", DbType.String, sd.User.Id);
                db.AddOutParameter(dbCmd, "@Roles", DbType.String, 255);
                if (DatabaseAccess.LogDatabaseAccess)
                    Log.Info(dbCmd);
                db.ExecuteNonQuery(dbCmd);
                roles = Convert.ToString(db.GetParameterValue(dbCmd, "@Roles"));
                Log.Info("Roles = " + roles);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            SessionData.Instance.User.LogId = 0;
        }

        if (roles == null)
        {
            sd.User.Id = null;
            sd.User.Name = null;
            sd.User.IsAuthenticated = false;
            return Redirect("/Account/AboutUs");
        }

        sd.User.IsAdmin = roles.Contains("Admin");
        sd.User.AllowFullSGA = sd.User.IsAdmin || roles.Contains("FullSGA");
        sd.User.AllowFullWCD = sd.User.IsAdmin || roles.Contains("FullWCD");
        sd.User.IsRetailUser = roles.Contains("Retail");

        ///////////////////////////////////////////////////////////////////
        // This is temporary, allowing us to set the Roles based on the
        // local groups.  Delete this when practical.

        if (!sd.User.IsAdmin && User.IsInRole(Permission.PeerAMidAdmin))
        {
            sd.User.IsAdmin = true;
            SetUserRole(sd.User.UserName, "Admin", true);
        }

        if (!sd.User.AllowFullSGA && User.IsInRole(Permission.AllowSgaFull))
        {
            sd.User.AllowFullSGA = true;
            SetUserRole(sd.User.UserName, "FullSGA", true);
        }

        if (!sd.User.AllowFullWCD && User.IsInRole(Permission.AllowWcdFull))
        {
            sd.User.AllowFullWCD = true;
            SetUserRole(sd.User.UserName, "FullWCD", true);
        }

        if (!sd.User.IsRetailUser && User.IsInRole(Permission.AllowRetailFull))
        {
            sd.User.IsRetailUser = true;
            SetUserRole(sd.User.UserName, "Retail", true);
        }

        ///////////////////////////////////////////////////////////////////

        sd.User.AllowFullSGA = sd.User.AllowFullSGA || sd.User.IsAdmin;
        sd.User.AllowFullWCD = sd.User.AllowFullWCD || sd.User.IsAdmin;

        var gsd = MvcApplication.GlobalStaticData;
        SessionData.GlobalStaticData = gsd;

        // Proceed to main application page
        return Redirect("/PeerAMid/Index");
    }


    private static void SetUserRole(string userName, string roleName, bool allowed)
    {
        try
        {
            var db = DbFactory.CreateDatabase();
            var dbCmd = db.GetStoredProcCommand("YS.Proc_SetUserRole");
            {
                db.AddInParameter(dbCmd, "@UserName", DbType.String, userName);
                db.AddInParameter(dbCmd, "@RoleName", DbType.String, roleName);
                db.AddInParameter(dbCmd, "@Allowed", DbType.Int32, allowed ? 1 : 0);
                if (DatabaseAccess.LogDatabaseAccess)
                    Log.Info(dbCmd);
                db.ExecuteNonQuery(dbCmd);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            SessionData.Instance.User.LogId = 0;
        }
    }

    /// <summary>
    ///     After click on logout button update such logout time in database via Proc_UpdateLoginTime and clear and reset
    ///     session from application.
    /// </summary>
    /// <returns></returns>
    public ActionResult Logout()
    {
        SessionData.SingleSignOnEnabled = false;
        SessionData.Instance.User.IsAuthenticated = false;
        RecordLogOut();
        Session.Abandon();
        Session.Clear();
        Session.RemoveAll();
        return Redirect("/Account/AboutUs");
    }

    /// <summary>
    ///     When application run request first come on this method.
    /// </summary>
    /// <returns></returns>
    public ActionResult AboutUs()
    {
        if (SessionData.SingleSignOnEnabled)
            return RedirectToAction("UserLogin");
        return View();
    }

    [AllowAnonymous]
    [HttpGet]
    public ActionResult Home()
    {
        return View();
    }

    private void RecordLogOut()
    {
    }
}
