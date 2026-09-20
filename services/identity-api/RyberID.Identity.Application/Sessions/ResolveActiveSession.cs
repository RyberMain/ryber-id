namespace RyberID.Identity.Application.Sessions;

public sealed class ResolveActiveSession(
    ISessionStore sessionStore,
    ISessionTokenService sessionTokenService,
    TimeProvider timeProvider)
{
    public async Task<SessionIdentity?> ExecuteAsync(
        string sessionToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sessionToken))
        {
            return null;
        }

        var tokenHash =
            sessionTokenService.Hash(
                sessionToken);

        var session =
            await sessionStore.GetByTokenHashAsync(
                tokenHash,
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
