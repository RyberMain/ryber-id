namespace RyberID.Identity.Application.Sessions;

public sealed class RevokeCurrentSession(
    ISessionStore sessionStore,
    TimeProvider timeProvider)
{
    public async Task ExecuteAsync(
        SessionIdentity identity,
        CancellationToken cancellationToken = default)
    {
        var session =
            await sessionStore.GetByIdAsync(
                identity.SessionId,
                cancellationToken)
            ?? throw new InvalidOperationException(
                "Session was not found.");

        if (session.UserId != identity.UserId)
        {
            throw new InvalidOperationException(
                "Session does not belong to the authenticated user.");
        }

        var revokedAtUtc =
            timeProvider.GetUtcNow();

        session.Revoke(
            revokedAtUtc);

        await sessionStore.UpdateAsync(
            session,
            cancellationToken);
    }
}
