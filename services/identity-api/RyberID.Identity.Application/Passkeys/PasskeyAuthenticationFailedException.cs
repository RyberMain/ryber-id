namespace RyberID.Identity.Application.Passkeys;

public sealed class PasskeyAuthenticationFailedException : Exception
{
    public PasskeyAuthenticationFailedException()
        : base("Passkey authentication failed.")
    {
    }

    public PasskeyAuthenticationFailedException(Exception innerException)
        : base("Passkey authentication failed.", innerException)
    {
    }
}
