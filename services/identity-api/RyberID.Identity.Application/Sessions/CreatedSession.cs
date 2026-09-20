namespace RyberID.Identity.Application.Sessions;

public sealed class CreatedSession
{
    public CreatedSession(
        SessionState session,
        string token)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        Session = session;
        Token = token;
    }

    public SessionState Session { get; }

    public string Token { get; }
}
