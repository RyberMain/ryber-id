namespace RyberID.Identity.Application.Sessions;

public sealed class SessionNotFoundException : Exception
{
    public SessionNotFoundException()
        : base("Session was not found.")
    {
    }
}
