namespace RyberID.Identity.Api.Configuration;

public static class SessionAuthenticationDefaults
{
    public const string Scheme =
        "RyberID.Session";

    public const string CookieName =
        "__Host-ryberid_session";

    public const string SessionIdClaim =
        "ryberid:session_id";
}
