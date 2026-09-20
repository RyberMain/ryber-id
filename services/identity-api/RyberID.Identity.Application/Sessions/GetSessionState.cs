using RyberID.Identity.Application.Authentication;

namespace RyberID.Identity.Application.Sessions;

public sealed class GetSessionState(
    ISessionStore sessionStore)
{
    public async Task<SessionState> ExecuteAsync(
        AuthenticatedIdentity identity,
        Guid sessionId,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        var session =
            await sessionStore.GetByIdAsync(
                sessionId,
                cancellationToken)
            ?? throw new InvalidOperationException(
                "Session was not found.");

        if (session.UserId != identity.UserId)
        {
            throw new InvalidOperationException(
                "Session does not belong to the authenticated user.");
        }

        return new SessionState(
            session.Id,
            session.UserId,
            session.CreatedAtUtc,
            session.ExpiresAtUtc,
            session.RevokedAtUtc,
            session.IsActive(nowUtc));
    }
}
