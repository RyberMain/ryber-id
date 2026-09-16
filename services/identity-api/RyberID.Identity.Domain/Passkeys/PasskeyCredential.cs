namespace RyberID.Identity.Domain.Passkeys;

public sealed class PasskeyCredential
{
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
        CredentialId = credentialId;
        PublicKey = publicKey;
        SignCount = signCount;
    }

    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public byte[] CredentialId { get; private set; } = [];

    public byte[] PublicKey { get; private set; } = [];

    public uint SignCount { get; private set; }

    public static PasskeyCredential Create(
        Guid userId,
        byte[] credentialId,
        byte[] publicKey,
        uint signCount)
    {
        return new PasskeyCredential(
            Guid.CreateVersion7(),
            userId,
            credentialId,
            publicKey,
            signCount);
    }

    public void UpdateSignCount(uint signCount)
    {
        SignCount = signCount;
    }
}