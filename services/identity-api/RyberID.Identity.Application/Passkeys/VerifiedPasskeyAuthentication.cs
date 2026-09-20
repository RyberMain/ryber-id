namespace RyberID.Identity.Application.Passkeys;

public sealed record VerifiedPasskeyAuthentication(
    Guid UserId,
    byte[] CredentialId,
    uint SignCount);
