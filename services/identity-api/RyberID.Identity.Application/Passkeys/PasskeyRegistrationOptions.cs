namespace RyberID.Identity.Application.Passkeys;

public sealed record PasskeyRegistrationOptions(
    string Json,
    ulong TimeoutMilliseconds);
