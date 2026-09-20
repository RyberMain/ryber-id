using RyberID.Identity.Application.Authentication;
using RyberID.Identity.Domain.Passkeys;

namespace RyberID.Identity.Application.Passkeys;

public sealed class CompletePasskeyAuthentication(
    IPasskeyAuthenticationStateStore stateStore,
    IPasskeyAuthenticationVerifier verifier,
    IPasskeyCredentialStore credentialStore)
{
    public async Task<AuthenticatedIdentity> ExecuteAsync(
        Guid ceremonyId,
        string assertionResponseJson,
        CancellationToken cancellationToken = default)
    {
        var optionsJson =
            await stateStore.ConsumeAsync(
                ceremonyId,
                cancellationToken)
            ?? throw new InvalidOperationException(
                "Passkey authentication ceremony was not found, has expired, or was already consumed.");

        var verified =
            await verifier.VerifyAsync(
                optionsJson,
                assertionResponseJson,
                cancellationToken);

        if (!PasskeyCredential.IsValidSignCountTransition(
                verified.StoredSignCount,
                verified.SignCount))
        {
            throw new InvalidOperationException(
                "Passkey signature counter did not advance.");
        }

        var signCountWasUpdated =
            await credentialStore.TryUpdateSignCountAsync(
                verified.CredentialRecordId,
                verified.StoredSignCount,
                verified.SignCount,
                cancellationToken);

        if (!signCountWasUpdated)
        {
            throw new InvalidOperationException(
                "Passkey credential changed during authentication.");
        }

        return new AuthenticatedIdentity(
            verified.UserId);
    }
}
