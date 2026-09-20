namespace RyberID.Identity.Application.Passkeys;

public sealed record VerifiedPasskeyAuthentication(
    Guid UserId,
    Guid CredentialRecordId,
    uint StoredSignCount,
    uint SignCount);
