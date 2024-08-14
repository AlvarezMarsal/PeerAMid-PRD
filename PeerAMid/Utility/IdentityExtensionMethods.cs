using System.Security.Principal;

namespace PeerAMid.Utility;

#nullable enable

public static class IdentityExtensionMethods
{
    /*
    public static string GetDomain(this IIdentity identity)
    {
        var s = identity?.Name;
        if (!string.IsNullOrEmpty(s))
        {
            var stop = s!.IndexOf("\\");
            return stop > -1 ? s.Substring(0, stop) : string.Empty;
        }

        return string.Empty;
    }
    */

    public static string? GetLogin(this IIdentity identity)
    {
        if (identity == null)
            return null;
        var name = identity!.Name;
        if (string.IsNullOrEmpty(name))
            return null;

        var slash = name.LastIndexOf("\\");
        if (slash == -1)
            return name;
        return name.Substring(slash + 1);
    }
}
