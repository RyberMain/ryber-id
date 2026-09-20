using RyberID.Identity.Application.Authentication;
using RyberID.Identity.Domain.Sessions;

namespace RyberID.Identity.Application.Sessions;

public sealed class CreateSession(
    ISessionStore sessionStore,
    ISessionTokenService sessionTokenService,
    ISessionLifetimePolicy lifetimePolicy,
    TimeProvider timeProvider)
{
    public async Task<CreatedSession> ExecuteAsync(
        AuthenticatedIdentity identity,
        CancellationToken cancellationToken = default)
    {
        var createdAtUtc =
            timeProvider.GetUtcNow();

        var lifetime =
            lifetimePolicy.GetLifetime();

        var expiresAtUtc =
            createdAtUtc.Add(lifetime);

        var token =
            sessionTokenService.Issue();

        var session =
            Session.Create(
                identity.UserId,
                token.Hash,
                createdAtUtc,
                expiresAtUtc);

        await sessionStore.AddAsync(
            session,
            cancellationToken);

        var state =
            new SessionState(
                session.Id,
                session.UserId,
                session.CreatedAtUtc,
                session.ExpiresAtUtc,
                session.RevokedAtUtc,
                session.IsActive(createdAtUtc));

        return new CreatedSession(
            state,
            token.Value);
    }
}
