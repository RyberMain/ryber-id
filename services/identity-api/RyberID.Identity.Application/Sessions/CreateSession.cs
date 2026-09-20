using RyberID.Identity.Application.Authentication;
using RyberID.Identity.Domain.Sessions;

namespace RyberID.Identity.Application.Sessions;

public sealed class CreateSession(
    ISessionStore sessionStore,
    ISessionLifetimePolicy lifetimePolicy,
    TimeProvider timeProvider)
{
    public async Task<SessionState> ExecuteAsync(
        AuthenticatedIdentity identity,
        CancellationToken cancellationToken = default)
    {
        var createdAtUtc =
            timeProvider.GetUtcNow();

        var lifetime =
            lifetimePolicy.GetLifetime();

        var expiresAtUtc =
            createdAtUtc.Add(lifetime);

        var session =
            Session.Create(
                identity.UserId,
                createdAtUtc,
                expiresAtUtc);

        await sessionStore.AddAsync(
            session,
            cancellationToken);

        return new SessionState(
            session.Id,
            session.UserId,
            session.CreatedAtUtc,
            session.ExpiresAtUtc,
            session.RevokedAtUtc,
            session.IsActive(createdAtUtc));
    }
}
