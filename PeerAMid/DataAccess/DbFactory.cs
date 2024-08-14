using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using PeerAMid.Support;
using PeerAMid.Utility;
using System.Configuration;

#nullable enable

namespace PeerAMid.DataAccess;

public static class DbFactory
{
    private static readonly string ConnectionString;

    static DbFactory()
    {
        var connectionStringName = ConfigurationManager.AppSettings.GetForThisMachine("Database");
        var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
        if (connectionString.Length == 0)
        {
            Log.Debug("Missing connection string for " + connectionStringName);
        }
        else if (connectionString[0] == '#')
        {
            connectionString = Decrypt(connectionString);
        }
        else
        {
            var parts = connectionString.Split(';');
            connectionString = "";
            foreach (var part in parts)
            {
                var equalsIndex = part.IndexOf('=');
                if (equalsIndex > 0)
                {
                    var value = part.Substring(equalsIndex + 1).Trim();
                    // If the value starts with '#', it's encrypted
                    if ((value.Length > 0) && (value[0] == '#'))
                        value = Decrypt(value, connectionStringName + ":");

                    var name = part.Substring(0, equalsIndex).Trim();
                    connectionString += name + "=" + value + ";";
                }
                else
                {
                    connectionString += part + ";";
                }
            }
        }

        ConnectionString = connectionString;
    }

    private static string Decrypt(string encrypted, string? possiblePrefix = null)
    {
        if (encrypted[0] == '#')
            encrypted = encrypted.Substring(1);

        // It's in base64, but we allow any terminating equals signs '='
        // to be omitted.  This might obscure things a bit.
        encrypted = (encrypted.Length % 4) switch
        {
            1 => encrypted + "===",
            2 => encrypted + "==",
            3 => encrypted + "=",
            _ => encrypted,
        };

        // Decrypt it
        var plain = CryptoHelper.Decrypt(encrypted);

        // we allow the password to be encrypted with the connectionStringName used as a prefix
        // so that using the same password on multiple databases won't be visible in the shared
        // config file
        if (!string.IsNullOrEmpty(possiblePrefix) && plain.StartsWith(possiblePrefix!))
            plain = plain.Substring(possiblePrefix!.Length);

        return plain;
    }

    public static Database CreateDatabase()
    {
        return new SqlDatabase(ConnectionString); //DatabaseFactory.CreateDatabase(ConnectionStringName);
    }
}
