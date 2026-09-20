namespace RyberID.Identity.Application.Passkeys;

public sealed record PasskeyRegistrationStart(
    Guid CeremonyId,
    string OptionsJson);
