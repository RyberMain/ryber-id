namespace RyberID.Identity.Api.Contracts.Sessions;

public sealed record CurrentSessionResponse(
    Guid UserId,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset ExpiresAtUtc);
