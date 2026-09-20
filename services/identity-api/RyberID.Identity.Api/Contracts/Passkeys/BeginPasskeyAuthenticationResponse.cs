using System.Text.Json;

namespace RyberID.Identity.Api.Contracts.Passkeys;

public sealed record BeginPasskeyAuthenticationResponse(
    Guid CeremonyId,
    JsonElement Options);
