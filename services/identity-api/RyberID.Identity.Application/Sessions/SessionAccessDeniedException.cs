namespace RyberID.Identity.Application.Sessions;

public sealed class SessionAccessDeniedException : Exception
{
    public SessionAccessDeniedException()
        : base("The session does not belong to the authenticated user.")
    {
    }
}
