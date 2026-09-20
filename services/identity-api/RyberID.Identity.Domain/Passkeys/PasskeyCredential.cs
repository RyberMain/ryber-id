namespace RyberID.Identity.Domain.Passkeys;

public sealed class PasskeyCredential
{
    private byte[] _credentialId = [];
    private byte[] _publicKey = [];

    private PasskeyCredential()
    {
    }

    private PasskeyCredential(
        Guid id,
        Guid userId,
        byte[] credentialId,
        byte[] publicKey,
        uint signCount)
    {
        Id = id;
        UserId = userId;
        _credentialId = credentialId.ToArray();
        _publicKey = publicKey.ToArray();
        SignCount = signCount;
    }

    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public byte[] CredentialId => _credentialId.ToArray();

    public byte[] PublicKey => _publicKey.ToArray();

    public uint SignCount { get; private set; }

    public static PasskeyCredential Create(
        Guid userId,
        byte[] credentialId,
        byte[] publicKey,
        uint signCount)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException(
                "Passkey user identifier cannot be empty.",
                nameof(userId));
        }

        ArgumentNullException.ThrowIfNull(credentialId);
        ArgumentNullException.ThrowIfNull(publicKey);

        if (credentialId.Length == 0)
        {
            throw new ArgumentException(
                "Passkey credential identifier cannot be empty.",
                nameof(credentialId));
        }

        if (publicKey.Length == 0)
        {
            throw new ArgumentException(
                "Passkey public key cannot be empty.",
                nameof(publicKey));
        }

        return new PasskeyCredential(
            Guid.CreateVersion7(),
            userId,
            credentialId,
            publicKey,
            signCount);
    }

    public static bool IsValidSignCountTransition(
        uint storedSignCount,
        uint receivedSignCount)
    {
        return
            storedSignCount == 0 ||
            receivedSignCount > storedSignCount;
    }
}
