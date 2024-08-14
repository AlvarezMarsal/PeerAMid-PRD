using System.Collections.Specialized;
using System.Configuration;


namespace PeerAMid.Utility;

#nullable enable

public static class NameValueCollectionExtensionMethods
{
    public static bool LogThis = true;

    public static string? GetForThisMachine(this NameValueCollection collection, string name, string? defaultValue = null)
    {
        var key = name + "-" + Environment.MachineName;
        if (LogThis) Log.Debug("Looking for: " + key);
        var result = collection.Get(name + "-" + Environment.MachineName);
        if (result == null)
        {
            if (LogThis) Log.Debug("Falling back to: " + name);
            result = collection.Get(name);
            if (result == null)
            {
                if (defaultValue == null)
                {
                    if (LogThis) Log.Debug($"Returning: default value <null> for {key}");
                }
                else if (defaultValue.Length == 0)
                {
                    if (LogThis) Log.Debug($"Returning: default value <empty> for {key}");
                }
                else
                {
                    if (LogThis) Log.Debug($"Returning default value {defaultValue} for {key}");
                }

                return defaultValue;
            }
            else if (result.Length == 0)
            {
                if (LogThis) Log.Debug($"Returning <empty> for {name}");
            }
            else
            {
                if (LogThis) Log.Debug($"Returning {result} for {name}");
            }
        }
        else if (result.Length == 0)
        {
            if (LogThis) Log.Debug($"Returning: <empty> for {key}");
        }
        else
        {
            if (LogThis) Log.Debug($"Returning {result} for {key}");
        }
        return result;
    }

    public static string? GetForThisMachineNoLog(this NameValueCollection collection, string name, string? defaultValue = null)
    {
        var key = name + "-" + Environment.MachineName;
        return ConfigurationManager.AppSettings.Get(key) ?? ConfigurationManager.AppSettings.Get(name) ?? defaultValue;
    }


    public static bool GetForThisMachine(this NameValueCollection collection, string name, bool defaultValue)
    {
        var value = GetForThisMachine(collection, name, null);
        if (string.IsNullOrEmpty(value))
            return defaultValue;
        if ((value![0] == 't') || (value[0] == 'T') || (value[0] == '1'))
            return true;
        if ((value![0] == 'f') || (value[0] == 'F') || (value[0] == '0'))
            return false;
        return defaultValue;
    }
}
