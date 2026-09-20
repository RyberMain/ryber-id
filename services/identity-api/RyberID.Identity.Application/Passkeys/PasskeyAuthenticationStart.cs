namespace RyberID.Identity.Application.Passkeys;

public sealed record PasskeyAuthenticationStart(
    Guid CeremonyId,
    string OptionsJson);
