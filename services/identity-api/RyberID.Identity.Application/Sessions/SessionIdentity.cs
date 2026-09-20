namespace RyberID.Identity.Application.Sessions;

public sealed record SessionIdentity(
    Guid SessionId,
    Guid UserId);
