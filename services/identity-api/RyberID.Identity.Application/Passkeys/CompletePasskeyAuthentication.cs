namespace RyberID.Identity.Application.Passkeys;

public sealed class CompletePasskeyAuthentication(
    IPasskeyAuthenticationStateStore stateStore,
    IPasskeyAuthenticationVerifier verifier,
    IPasskeyCredentialStore credentialStore)
{
    public async Task<Guid> ExecuteAsync(
        Guid ceremonyId,
        string assertionResponseJson,
        CancellationToken cancellationToken = default)
    {
        var optionsJson =
            await stateStore.GetAsync(
                ceremonyId,
                cancellationToken)
            ?? throw new InvalidOperationException(
                "Passkey authentication ceremony was not found or has expired.");

        var verified =
            await verifier.VerifyAsync(
                optionsJson,
                assertionResponseJson,
                cancellationToken);

        var credential =
            await credentialStore.GetByCredentialIdAsync(
                verified.CredentialId,
                cancellationToken)
            ?? throw new InvalidOperationException(
                "Passkey credential was not found.");

        if (credential.UserId != verified.UserId)
        {
            throw new InvalidOperationException(
                "Passkey credential does not belong to the verified user.");
        }

        credential.UpdateSignCount(
            verified.SignCount);

        await credentialStore.UpdateAsync(
            credential,
            cancellationToken);

        await stateStore.RemoveAsync(
            ceremonyId,
            cancellationToken);

        return verified.UserId;
    }
}
