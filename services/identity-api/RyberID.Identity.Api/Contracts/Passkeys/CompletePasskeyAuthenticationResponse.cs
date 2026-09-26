namespace RyberID.Identity.Api.Contracts.Passkeys;

public sealed record CompletePasskeyAuthenticationResponse(
    Guid UserId,
    DateTimeOffset SessionExpiresAtUtc);
