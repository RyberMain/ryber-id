using System.Security.Claims;
using RyberID.Identity.Api.Configuration;
using RyberID.Identity.Application.Sessions;

namespace RyberID.Identity.Api.Middleware;

public static class SessionPrincipalExtensions
{
    public static SessionIdentity
        GetRequiredSessionIdentity(
            this ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(
            principal);

        var userIdValue =
            principal
                .FindFirst(
                    ClaimTypes.NameIdentifier)
                ?.Value;

        var sessionIdValue =
            principal
                .FindFirst(
                    SessionAuthenticationDefaults.SessionIdClaim)
                ?.Value;

        if (!Guid.TryParse(
                userIdValue,
                out var userId))
        {
            throw new InvalidOperationException(
                "Authenticated principal does not contain a valid user identifier.");
        }

        if (!Guid.TryParse(
                sessionIdValue,
                out var sessionId))
        {
            throw new InvalidOperationException(
                "Authenticated principal does not contain a valid session identifier.");
        }

        return new SessionIdentity(
            sessionId,
            userId);
    }
}
