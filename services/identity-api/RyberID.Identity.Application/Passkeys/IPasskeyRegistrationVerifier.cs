namespace RyberID.Identity.Application.Passkeys;

public interface IPasskeyRegistrationVerifier
{
    Task<VerifiedPasskeyRegistration> VerifyAsync(
        string optionsJson,
        string attestationResponseJson,
        CancellationToken cancellationToken);
}
