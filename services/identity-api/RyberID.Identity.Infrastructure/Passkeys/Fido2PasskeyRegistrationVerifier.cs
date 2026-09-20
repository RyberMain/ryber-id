using System.Text.Json;
using Fido2NetLib;
using RyberID.Identity.Application.Passkeys;

namespace RyberID.Identity.Infrastructure.Passkeys;

internal sealed class Fido2PasskeyRegistrationVerifier(
    IFido2 fido2,
    IPasskeyCredentialStore credentialStore)
    : IPasskeyRegistrationVerifier
{
    public async Task<VerifiedPasskeyRegistration> VerifyAsync(
        string optionsJson,
        string attestationResponseJson,
        CancellationToken cancellationToken)
    {
        var originalOptions =
            CredentialCreateOptions.FromJson(
                optionsJson);

        var attestationResponse =
            JsonSerializer.Deserialize<
                AuthenticatorAttestationRawResponse>(
                    attestationResponseJson)
            ?? throw new InvalidOperationException(
                "Passkey attestation response is invalid.");

        IsCredentialIdUniqueToUserAsyncDelegate
            credentialIdIsUnique =
                async (args, token) =>
                    !await credentialStore
                        .ExistsByCredentialIdAsync(
                            args.CredentialId,
                            token);

        var credential =
            await fido2.MakeNewCredentialAsync(
                new MakeNewCredentialParams
                {
                    AttestationResponse =
                        attestationResponse,

                    OriginalOptions =
                        originalOptions,

                    IsCredentialIdUniqueToUserCallback =
                        credentialIdIsUnique
                },
                cancellationToken);

        return new VerifiedPasskeyRegistration(
            credential.Id,
            credential.PublicKey,
            credential.User.Id,
            credential.SignCount);
    }
}
