namespace PeerAMid.Business;

#nullable enable

public static class Permission
{
    public const string PeerAMidUser = "PeerAMidUsersSEC";
    public const string PeerAMidAdmin = "PeerAMidAdmin";
    public const string AllowSgaFull = "PeerAMidFeature-SGA";
    public const string AllowWcdFull = "PeerAMidFeature-WC";
    public const string AllowRetailFull = "PeerAMidFeature-SGA"; // Currently, anyone who can do SGA can do retail
}
