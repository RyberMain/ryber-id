using RyberID.Identity.Application.Authentication;
using RyberID.Identity.Domain.Sessions;

namespace RyberID.Identity.Application.Sessions;

public sealed class CreateSession(
    ISessionStore sessionStore)
{
    public async Task<SessionState> ExecuteAsync(
        AuthenticatedIdentity identity,
        DateTimeOffset createdAtUtc,
        DateTimeOffset expiresAtUtc,
        CancellationToken cancellationToken = default)
    {
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
