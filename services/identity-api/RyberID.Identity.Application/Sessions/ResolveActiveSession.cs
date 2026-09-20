namespace RyberID.Identity.Application.Sessions;

public sealed class ResolveActiveSession(
    ISessionStore sessionStore)
{
    public async Task<SessionIdentity?> ExecuteAsync(
        Guid sessionId,
        DateTimeOffset nowUtc,
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

        if (!session.IsActive(nowUtc))
        {
            return null;
        }

        return new SessionIdentity(
            session.Id,
            session.UserId);
    }
}
