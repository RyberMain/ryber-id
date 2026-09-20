using System.Text.Json;

namespace RyberID.Identity.Api.Contracts.Passkeys;

public sealed record BeginPasskeyRegistrationResponse(
    Guid CeremonyId,
    JsonElement Options);
