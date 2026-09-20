namespace RyberID.Identity.Domain.Sessions;

public sealed class Session
{
    public const int TokenHashSizeInBytes = 32;

    private byte[] _tokenHash = [];

    private Session()
    {
    }

    private Session(
        Guid id,
        Guid userId,
        byte[] tokenHash,
        DateTimeOffset createdAtUtc,
        DateTimeOffset expiresAtUtc)
    {
        Id = id;
        UserId = userId;
        _tokenHash = tokenHash.ToArray();
        CreatedAtUtc = createdAtUtc;
        ExpiresAtUtc = expiresAtUtc;
    }

    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public byte[] TokenHash => _tokenHash.ToArray();

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public DateTimeOffset ExpiresAtUtc { get; private set; }

    public DateTimeOffset? RevokedAtUtc { get; private set; }

    public bool IsActive(
        DateTimeOffset nowUtc)
    {
        return
            RevokedAtUtc is null &&
            nowUtc >= CreatedAtUtc &&
            nowUtc < ExpiresAtUtc;
    }

    public static Session Create(
        Guid userId,
        byte[] tokenHash,
        DateTimeOffset createdAtUtc,
        DateTimeOffset expiresAtUtc)
    {
        ArgumentNullException.ThrowIfNull(
            tokenHash);

        if (userId == Guid.Empty)
        {
            throw new ArgumentException(
                "Session user identifier cannot be empty.",
                nameof(userId));
        }

        if (tokenHash.Length != TokenHashSizeInBytes)
        {
            throw new ArgumentException(
                $"Session token hash must contain {TokenHashSizeInBytes} bytes.",
                nameof(tokenHash));
        }

        if (expiresAtUtc <= createdAtUtc)
        {
            throw new ArgumentOutOfRangeException(
                nameof(expiresAtUtc),
                "Session expiration must be later than its creation time.");
        }

        return new Session(
            Guid.CreateVersion7(),
            userId,
            tokenHash,
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

        if (revokedAtUtc < CreatedAtUtc)
        {
            throw new ArgumentOutOfRangeException(
                nameof(revokedAtUtc),
                "Session revocation cannot precede its creation time.");
        }

        RevokedAtUtc = revokedAtUtc;
    }
}
