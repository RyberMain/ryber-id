namespace RyberID.Identity.Application.Sessions;

public sealed class ResolveActiveSession(
    ISessionStore sessionStore,
    TimeProvider timeProvider)
{
    public async Task<SessionIdentity?> ExecuteAsync(
        Guid sessionId,
        CancellationToken cancellationToken = default)
    {
        var session =
            await sessionStore.GetByIdAsync(
                sessionId,
                cancellationToken);

        if (session is null)
        {
            return null;
        }

        var nowUtc =
            timeProvider.GetUtcNow();

        if (!session.IsActive(nowUtc))
        {
            return null;
        }

        return new SessionIdentity(
            session.Id,
            session.UserId);
    }
}
