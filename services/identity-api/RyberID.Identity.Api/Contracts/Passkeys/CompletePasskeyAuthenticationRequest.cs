using System.Text.Json;

namespace RyberID.Identity.Api.Contracts.Passkeys;

public sealed record CompletePasskeyAuthenticationRequest(
    Guid CeremonyId,
    JsonElement AssertionResponse);
