using RyberID.Identity.Application.Authentication;

namespace RyberID.Identity.Application.Sessions;

public sealed class GetSessionState(
    ISessionStore sessionStore,
    TimeProvider timeProvider)
{
    public async Task<SessionState> ExecuteAsync(
        SessionIdentity identity,
        CancellationToken cancellationToken = default)
    {
        var session =
            await sessionStore.GetByIdAsync(
                identity.SessionId,
                cancellationToken)
            ?? throw new SessionNotFoundException();

        if (session.UserId != identity.UserId)
        {
            throw new SessionAccessDeniedException();
        }

        var nowUtc =
            timeProvider.GetUtcNow();

        return new SessionState(
            session.Id,
            session.UserId,
            session.CreatedAtUtc,
            session.ExpiresAtUtc,
            session.RevokedAtUtc,
            session.IsActive(nowUtc));
    }
}
