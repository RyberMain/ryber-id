using RyberID.Identity.Domain.Passkeys;

namespace RyberID.Identity.Application.Passkeys;

public sealed class CompletePasskeyRegistration(
    IPasskeyRegistrationStateStore stateStore,
    IPasskeyRegistrationVerifier verifier,
    IPasskeyUserHandleStore userHandleStore,
    IPasskeyCredentialStore credentialStore)
{
    public async Task<Guid> ExecuteAsync(
        Guid userId,
        Guid ceremonyId,
        string attestationResponseJson,
        CancellationToken cancellationToken = default)
    {
        var optionsJson =
            await stateStore.ConsumeAsync(
                ceremonyId,
                cancellationToken)
            ?? throw new InvalidOperationException(
                "Passkey registration ceremony was not found, has expired, or was already consumed.");

        var userHandle =
            await userHandleStore.GetByUserIdAsync(
                userId,
                cancellationToken)
            ?? throw new InvalidOperationException(
                "Passkey user handle was not found.");

        var verified =
            await verifier.VerifyAsync(
                optionsJson,
                attestationResponseJson,
                cancellationToken);

        if (!userHandle.Value.SequenceEqual(
                verified.UserHandle))
        {
            throw new InvalidOperationException(
                "Passkey registration user handle does not match the requested user.");
        }

        var credential =
            PasskeyCredential.Create(
                userId,
                verified.CredentialId,
                verified.PublicKey,
                verified.SignCount);

        await credentialStore.AddAsync(
            credential,
            cancellationToken);

        return credential.Id;
    }
}
