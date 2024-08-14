using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace CheckAccessRights
{
    internal class Program
    {
        private const string ComServerExec = "ComServerExec";
        private const string IisIUsrs = "IIS_IUSRS";
        private const string IUsr = "IUSR";
        private const string NetworkService = "NETWORK SERVICE";

        private static void Main(string[] args)
        {
            using (var identity = WindowsIdentity.GetCurrent())
            {
                var principal = new WindowsPrincipal(identity);
                if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
                {
                    Console.WriteLine("You must run this program as an Administrator.");
                    return;
                }
            }

            var problems = 0;

            if (!DirectoryExists(@"C:\Windows\SysWOW64\config\systemprofile\Desktop"))
                problems++;

            if (!LocalAccountExists(ComServerExec))
            {
                problems++;
            }
            else
            {
                /*
                using (var domainContext = new PrincipalContext(ContextType.Domain, Environment.MachineName))
                {
                    using (var foundUser = UserPrincipal.FindByIdentity(domainContext, IdentityType.SamAccountName, ComServerExec))
                    {
                        if (!foundUser.Enabled.HasValue)
                        {
                            Console.WriteLine($"Could not determine if the {ComServerExec} account is enabled.");
                        }
                        else if (!foundUser.Enabled.Value)
                        {
                            Console.WriteLine($"The {ComServerExec} account is disabled.");
                            problems++;
                        }
                    }
                }
                */

                /*
                if (!LocalUserIsInGroup(ComServerExec, "Distributed Com Users"))
                    problems++;
                if (!LocalUserIsInGroup(ComServerExec, "Administrators"))
                    problems++;
                if (!LocalUserIsInGroup(ComServerExec, "Remote Desktop Users"))
                    problems++;
                if (!LocalUserIsInGroup(ComServerExec, "IIS_IUSRS"))
                    problems++;
                */

                if (!LocalUserHasAccessToDirectory(ComServerExec, @"C:\Windows\SysWOW64\config\systemprofile\Desktop"))
                    ++problems;
                if (!LocalUserHasAccessToDirectory(ComServerExec, @"C:\www"))
                    ++problems;
            }

            if (!LocalGroupExists(IisIUsrs))
            {
                ++problems;
            }
            else
            {
                if (!LocalUserHasAccessToDirectory(IisIUsrs, @"C:\www"))
                    ++problems;
            }

            if (!LocalGroupExists(IUsr))
            {
                ++problems;
            }
            else
            {
                if (!LocalUserHasAccessToDirectory(IUsr, @"C:\www"))
                    ++problems;
            }

            if (!LocalUserHasAccessToDirectory(NetworkService, @"C:\www"))
                ++problems;

            if (problems == 0)
                Console.WriteLine("No problems found");
            else if (problems == 1)
                Console.WriteLine("1 problem found");
            else
                Console.WriteLine($"{problems} problems found");

            Console.Write("Hit any key to continue.");
            Console.ReadKey();
            Console.WriteLine();
        }

        private static bool DirectoryExists(string directory)
        {
            try
            {
                if (Directory.Exists(directory))
                    return true;
                Console.WriteLine($"Directory does not exist: {directory}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not determine if directory exists: {directory}");
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private static bool LocalAccountExists(string name)
        {
            try
            {
                NTAccount account = new NTAccount(Environment.MachineName, name);
                SecurityIdentifier sid = (SecurityIdentifier)account.Translate(typeof(SecurityIdentifier));
                if (sid.IsAccountSid())
                    return true;
                Console.WriteLine($"Local user account does not exist: {name}");
                return false;
            }
            catch (IdentityNotMappedException)
            {
                Console.WriteLine($"Local user account does not exist: {name}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not determine if local user account exists: {name}");
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private static bool LocalGroupExists(string name)
        {
            try
            {
                NTAccount account = new NTAccount(Environment.MachineName, name);
                //SecurityIdentifier sid = (SecurityIdentifier)account.Translate(typeof(SecurityIdentifier));
                //if (!sid.IsAccountSid())
                return true;
                //Console.WriteLine($"Local group does not exist: {name}");
                //return false;
            }
            catch (IdentityNotMappedException)
            {
                Console.WriteLine($"Local groupdoes not exist: {name}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not determine if local group exists: {name}");
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /*
        private static bool LocalUserIsInGroup(string user, string group)
        {
            try
            {
                using (var identity = new WindowsIdentity(Environment.MachineName, user))
                {
                    foreach (var g in identity.Groups)
                    {
                        var translated = g.Translate(typeof(NTAccount));
                        Debug.WriteLine(translated.Value);
                        if (group.Equals(translated.Value, StringComparison.CurrentCultureIgnoreCase))
                            return true;
                    }
                    Console.WriteLine($"The {user} account is not a member of {group}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not determine if {user} account is a member of {group}");
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        */

        private static bool LocalUserHasAccessToDirectory(string user, string directory)
        {
            try
            {
                var accessControlList = Directory.GetAccessControl(directory);
                if (accessControlList == null)
                    return false;
                var accessRules = accessControlList.GetAccessRules(true, true, typeof(SecurityIdentifier));
                if (accessRules == null)
                    return false;

                FileSystemRights rights = 0;
                foreach (FileSystemAccessRule rule in accessRules)
                {
                    if (rule.AccessControlType == AccessControlType.Allow)
                        rights |= rule.FileSystemRights;
                    else
                        rights &= ~rule.FileSystemRights;
                }

                if ((rights & FileSystemRights.FullControl) == FileSystemRights.FullControl)
                    return true;

                Console.WriteLine($"The {user} account does not have full control of {directory}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not determine if {user} account has full control of {directory}");
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}