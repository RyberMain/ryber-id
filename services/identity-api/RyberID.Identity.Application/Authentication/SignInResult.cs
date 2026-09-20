using RyberID.Identity.Application.Sessions;

namespace RyberID.Identity.Application.Authentication;

public sealed record SignInResult(
    Guid UserId,
    SessionState Session);
