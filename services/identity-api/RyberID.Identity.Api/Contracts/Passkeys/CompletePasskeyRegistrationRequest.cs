using System.Text.Json;

namespace RyberID.Identity.Api.Contracts.Passkeys;

public sealed record CompletePasskeyRegistrationRequest(
    Guid CeremonyId,
    JsonElement AttestationResponse);
