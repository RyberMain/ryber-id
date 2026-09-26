namespace RyberID.Identity.Application.Passkeys;

public sealed class PasskeyCeremonyUnavailableException : Exception
{
    public PasskeyCeremonyUnavailableException()
        : base("The passkey ceremony was not found, has expired, or was already consumed.")
    {
    }
}
