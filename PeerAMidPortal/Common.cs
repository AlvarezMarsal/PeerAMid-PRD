using PeerAMid.Core;
using System;
using System.Configuration;

#nullable enable

namespace YardStickPortal;

public class CommonFunc
{
    public static int GetCurrentFy(IPeerAMidCore iPeerAMid)
    {
        return iPeerAMid.GetCurrentFinancialYear();
    }
}

public class PeerAMidAuthType
{
    public static string Cognito = "Cognito";
}

public class Config
{
    public static string PeerAMidAuthType
    {
        get
        {
            try
            {
                return ConfigurationManager.AppSettings["PeerAMidAuthType"];
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}

