using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace PeerAMid.Utility;


/////////////////////////////////////////////////////////////////////////
/// <summary>
/// Impersonation of a user. Allows to execute code under another
/// user context.
/// Please note that the account that instantiates the Impersonator class
/// needs to have the 'Act as part of operating system' privilege set.
/// </summary>
/// <remarks>	
/// This class is based on the information in the Microsoft knowledge base
/// article http://support.microsoft.com/default.aspx?scid=kb;en-us;Q306158
/// 
/// Encapsulate an instance into a using-directive like e.g.:
/// 
///		...
///		using ( new Impersonator( "myUsername", "myDomainname", "myPassword" ) )
///		{
///			...
///			[code that executes under the new context]
///			...
///		}
///		...
/// 
/// Please contact the author Uwe Keim (mailto:uwe.keim@zeta-software.de)
/// for questions regarding this class.
/// </remarks>
public sealed class Impersonator : IDisposable
{
    private WindowsImpersonationContext? impersonationContext = null;

    /// <summary>
    /// Constructor. Starts the impersonation with the given credentials.
    /// Please note that the account that instantiates the Impersonator class
    /// needs to have the 'Act as part of operating system' privilege set.
    /// </summary>
    /// <param name="userName">The name of the user to act as.</param>
    /// <param name="domainName">The domain name of the user to act as.</param>
    /// <param name="password">The password of the user to act as.</param>
    public Impersonator(string userName, string domainName, string password)
    {
        if (!ImpersonateValidUser(userName, domainName, password))
            throw new Win32Exception(Marshal.GetLastWin32Error());
    }


    public void Dispose()
    {
        UndoImpersonation();
    }

    /// <summary>
    /// Does the actual impersonation.
    /// </summary>
    /// <param name="userName">The name of the user to act as.</param>
    /// <param name="domainName">The domain name of the user to act as.</param>
    /// <param name="password">The password of the user to act as.</param>
    private bool ImpersonateValidUser(string userName, string domain, string password)
    {
        var token = IntPtr.Zero;
        var tokenDuplicate = IntPtr.Zero;

        try
        {
            if (NativeMethods.RevertToSelf())
            {
                if (NativeMethods.LogonUser(
                    userName,
                    domain,
                    password,
                    NativeMethods.LOGON32_LOGON_NETWORK,
                    NativeMethods.LOGON32_PROVIDER_DEFAULT,
                    ref token) != 0)
                {
                    if (NativeMethods.DuplicateToken(token, 2, ref tokenDuplicate) != 0)
                    {
                        var tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
                        impersonationContext = tempWindowsIdentity.Impersonate();
                    }
                    else
                    {
                        Log.Error("DuplicateToken failed with " + Marshal.GetLastWin32Error());
                        return false;
                        //throw new Win32Exception( Marshal.GetLastWin32Error() );
                    }
                }
                else
                {
                    Log.Error("LogonUser failed with " + Marshal.GetLastWin32Error());
                    return false;
                    // throw new Win32Exception( Marshal.GetLastWin32Error() );
                }
            }
            else
            {
                Log.Error("RevertToSelf failed with " + Marshal.GetLastWin32Error());
                return false;
                //throw new Win32Exception( Marshal.GetLastWin32Error() );
            }
        }
        finally
        {
            if (token != IntPtr.Zero)
            {
                NativeMethods.CloseHandle(token);
            }
            if (tokenDuplicate != IntPtr.Zero)
            {
                NativeMethods.CloseHandle(tokenDuplicate);
            }
        }

        return true;
    }

    /// <summary>
    /// Reverts the impersonation.
    /// </summary>
    private void UndoImpersonation()
    {
        if (impersonationContext != null)
        {
            impersonationContext.Undo();
            impersonationContext.Dispose();
            impersonationContext = null;
        }
    }



}
