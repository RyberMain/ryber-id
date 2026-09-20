namespace RyberID.Identity.Application.Passkeys;

public sealed record PasskeyAuthenticationOptions(
    string Json,
    ulong TimeoutMilliseconds);
