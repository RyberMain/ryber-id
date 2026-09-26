using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using RyberID.Identity.Api.Configuration;
using RyberID.Identity.Api.Contracts.Passkeys;
using RyberID.Identity.Application.Authentication;
using RyberID.Identity.Application.Passkeys;

namespace RyberID.Identity.Api.Endpoints;

public static class PasskeyAuthenticationEndpoints
{
    public static RouteGroupBuilder
        MapPasskeyAuthenticationEndpoints(
            this RouteGroupBuilder api)
    {
        var group =
            api.MapGroup(
                    "/passkeys/authentication")
                .WithTags(
                    "Passkey Authentication");

        group.MapPost(
                "/begin",
                BeginAsync)
            .AllowAnonymous()
            .WithName(
                "BeginPasskeyAuthentication");

        group.MapPost(
                "/complete",
                CompleteAsync)
            .AllowAnonymous()
            .WithName(
                "CompletePasskeyAuthentication");

        return api;
    }

    private static async Task<IResult> BeginAsync(
        BeginPasskeyAuthentication beginAuthentication,
        CancellationToken cancellationToken)
    {
        var start =
            await beginAuthentication.ExecuteAsync(
                cancellationToken);

        using var optionsDocument =
            JsonDocument.Parse(
                start.OptionsJson);

        var response =
            new BeginPasskeyAuthenticationResponse(
                start.CeremonyId,
                optionsDocument.RootElement.Clone());

        return Results.Ok(
            response);
    }

    private static async Task<IResult> CompleteAsync(
        CompletePasskeyAuthenticationRequest request,
        SignInWithPasskey signInWithPasskey,
        SessionCookieManager cookieManager,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (request.CeremonyId == Guid.Empty)
        {
            return Results.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    ["ceremonyId"] =
                        new[]
                        {
                            "A valid ceremonyId is required."
                        }
                });
        }

        if (request.AssertionResponse.ValueKind is
            JsonValueKind.Undefined or
            JsonValueKind.Null)
        {
            return Results.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    ["assertionResponse"] =
                        new[]
                        {
                            "The Passkey assertion response is required."
                        }
                });
        }

        var signInResult =
            await signInWithPasskey.ExecuteAsync(
                request.CeremonyId,
                request.AssertionResponse.GetRawText(),
                cancellationToken);

        cookieManager.Issue(
            httpContext.Response,
            signInResult.SessionToken);

        var response =
            new CompletePasskeyAuthenticationResponse(
                signInResult.UserId,
                signInResult.Session.ExpiresAtUtc);

        return Results.Ok(
            response);
    }
}
