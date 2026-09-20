namespace RyberID.Identity.Domain.Sessions;

public sealed class Session
{
    private Session()
    {
    }

    private Session(
        Guid id,
        Guid userId,
        DateTimeOffset createdAtUtc,
        DateTimeOffset expiresAtUtc)
    {
        Id = id;
        UserId = userId;
        CreatedAtUtc = createdAtUtc;
        ExpiresAtUtc = expiresAtUtc;
    }

    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public DateTimeOffset ExpiresAtUtc { get; private set; }

    public DateTimeOffset? RevokedAtUtc { get; private set; }

    public bool IsActive(DateTimeOffset nowUtc)
    {
        return
            RevokedAtUtc is null &&
            nowUtc < ExpiresAtUtc;
    }

    public static Session Create(
        Guid userId,
        DateTimeOffset createdAtUtc,
        DateTimeOffset expiresAtUtc)
    {
        if (expiresAtUtc <= createdAtUtc)
        {
            throw new ArgumentOutOfRangeException(
                nameof(expiresAtUtc),
                "Session expiration must be later than its creation time.");
        }

        return new Session(
            Guid.CreateVersion7(),
            userId,
            createdAtUtc,
            expiresAtUtc);
    }

    public void Revoke(
        DateTimeOffset revokedAtUtc)
    {
        if (RevokedAtUtc is not null)
        {
            return;
        }

        RevokedAtUtc = revokedAtUtc;
    }
}
