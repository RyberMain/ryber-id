using RyberID.Identity.Application.Sessions;

namespace RyberID.Identity.Application.Authentication;

public sealed class SignInResult
{
    public SignInResult(
        Guid userId,
        SessionState session,
        string sessionToken)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException(
                "Authenticated user identifier cannot be empty.",
                nameof(userId));
        }

        ArgumentNullException.ThrowIfNull(session);
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionToken);

        if (session.UserId != userId)
        {
            throw new ArgumentException(
                "Session does not belong to the authenticated user.",
                nameof(session));
        }

        UserId = userId;
        Session = session;
        SessionToken = sessionToken;
    }

    public Guid UserId { get; }

    public SessionState Session { get; }

    public string SessionToken { get; }
}
