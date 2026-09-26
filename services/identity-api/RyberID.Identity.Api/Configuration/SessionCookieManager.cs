using Microsoft.AspNetCore.Http;

namespace RyberID.Identity.Api.Configuration;

public sealed class SessionCookieManager
{
    public void Issue(
        HttpResponse response,
        string sessionToken)
    {
        ArgumentNullException.ThrowIfNull(response);
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionToken);

        response.Cookies.Append(
            SessionAuthenticationDefaults.CookieName,
            sessionToken,
            CreateOptions());
    }

    public void Delete(
        HttpResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);

        response.Cookies.Delete(
            SessionAuthenticationDefaults.CookieName,
            CreateOptions());
    }

    private static CookieOptions CreateOptions()
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/",
            IsEssential = true
        };
    }
}
