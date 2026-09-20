namespace RyberID.Identity.Application.Sessions;

public sealed class RevokeCurrentSession(
    ISessionStore sessionStore)
{
    public async Task ExecuteAsync(
        SessionIdentity identity,
        DateTimeOffset revokedAtUtc,
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

        session.Revoke(
            revokedAtUtc);

        await sessionStore.UpdateAsync(
            session,
            cancellationToken);
    }
}
