namespace RyberID.Identity.Application.Sessions;

public sealed record SessionState(
    Guid SessionId,
    Guid UserId,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset ExpiresAtUtc,
    DateTimeOffset? RevokedAtUtc,
    bool IsActive);
