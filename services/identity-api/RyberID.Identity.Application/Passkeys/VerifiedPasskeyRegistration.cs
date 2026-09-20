namespace RyberID.Identity.Application.Passkeys;

public sealed record VerifiedPasskeyRegistration(
    byte[] CredentialId,
    byte[] PublicKey,
    byte[] UserHandle,
    uint SignCount);
