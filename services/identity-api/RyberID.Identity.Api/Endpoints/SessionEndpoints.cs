using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using RyberID.Identity.Api.Configuration;
using RyberID.Identity.Api.Contracts.Sessions;
using RyberID.Identity.Api.Middleware;
using RyberID.Identity.Application.Sessions;

namespace RyberID.Identity.Api.Endpoints;

public static class SessionEndpoints
{
    public static RouteGroupBuilder
        MapSessionEndpoints(
            this RouteGroupBuilder api)
    {
        var group =
            api.MapGroup(
                    "/sessions")
                .WithTags(
                    "Sessions")
                .RequireAuthorization();

        group.MapGet(
                "/current",
                GetCurrentAsync)
            .WithName(
                "GetCurrentSession");

        group.MapDelete(
                "/current",
                RevokeCurrentAsync)
            .WithName(
                "RevokeCurrentSession");

        return api;
    }

    private static async Task<IResult> GetCurrentAsync(
        HttpContext httpContext,
        GetSessionState getSessionState,
        CancellationToken cancellationToken)
    {
        var sessionIdentity =
            httpContext.User
                .GetRequiredSessionIdentity();

        var session =
            await getSessionState.ExecuteAsync(
                sessionIdentity,
                cancellationToken);

        var response =
            new CurrentSessionResponse(
                session.UserId,
                session.CreatedAtUtc,
                session.ExpiresAtUtc);

        return Results.Ok(
            response);
    }

    private static async Task<IResult> RevokeCurrentAsync(
        HttpContext httpContext,
        RevokeCurrentSession revokeCurrentSession,
        SessionCookieManager cookieManager,
        CancellationToken cancellationToken)
    {
        var sessionIdentity =
            httpContext.User
                .GetRequiredSessionIdentity();

        await revokeCurrentSession.ExecuteAsync(
            sessionIdentity,
            cancellationToken);

        cookieManager.Delete(
            httpContext.Response);

        return Results.NoContent();
    }
}
