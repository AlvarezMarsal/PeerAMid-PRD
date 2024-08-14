using PeerAMid.Business;
using System.Security.Principal;

namespace PeerAMid.Utility;

public static class PrincipalExtensionMethods
{
    public static bool IsPeerAMidUser(this IPrincipal principal)
    {
        bool isGroupMember;
        try
        {
            isGroupMember = principal.IsInRole(Permission.PeerAMidUser);
        }
        catch (Exception ex)
        {
            isGroupMember = false;
            Log.Warn($"{ex} {principal.Identity.Name} {Permission.PeerAMidUser}");
        }

#if DEBUG
        isGroupMember = true;
#endif
        return isGroupMember;
    }
}
