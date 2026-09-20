namespace RyberID.Identity.Application.Passkeys;

public interface IPasskeyAuthenticationVerifier
{
    Task<VerifiedPasskeyAuthentication> VerifyAsync(
        string optionsJson,
        string assertionResponseJson,
        CancellationToken cancellationToken);
}
