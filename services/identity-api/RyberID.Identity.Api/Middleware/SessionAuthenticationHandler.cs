using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using RyberID.Identity.Api.Configuration;
using RyberID.Identity.Application.Sessions;

namespace RyberID.Identity.Api.Middleware;

public sealed class SessionAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    ResolveActiveSession resolveActiveSession,
    SessionCookieManager cookieManager)
    : AuthenticationHandler<AuthenticationSchemeOptions>(
        options,
        logger,
        encoder)
{
    protected override async Task<AuthenticateResult>
        HandleAuthenticateAsync()
    {
        if (!Request.Cookies.TryGetValue(
                SessionAuthenticationDefaults.CookieName,
                out var sessionToken) ||
            string.IsNullOrWhiteSpace(sessionToken))
        {
            return AuthenticateResult.NoResult();
        }

        var sessionIdentity =
            await resolveActiveSession.ExecuteAsync(
                sessionToken,
                Context.RequestAborted);

        if (sessionIdentity is null)
        {
            cookieManager.Delete(
                Response);

            return AuthenticateResult.Fail(
                "The session is invalid, expired, or revoked.");
        }

        var claims =
            new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    sessionIdentity.UserId.ToString()),

                new Claim(
                    SessionAuthenticationDefaults.SessionIdClaim,
                    sessionIdentity.SessionId.ToString())
            };

        var claimsIdentity =
            new ClaimsIdentity(
                claims,
                Scheme.Name);

        var principal =
            new ClaimsPrincipal(
                claimsIdentity);

        var ticket =
            new AuthenticationTicket(
                principal,
                Scheme.Name);

        return AuthenticateResult.Success(
            ticket);
    }
}
