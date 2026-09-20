using System.Text.Json;
using Fido2NetLib;
using RyberID.Identity.Application.Passkeys;

namespace RyberID.Identity.Infrastructure.Passkeys;

internal sealed class Fido2PasskeyAuthenticationVerifier(
    IFido2 fido2,
    IPasskeyCredentialStore credentialStore,
    IPasskeyUserHandleStore userHandleStore)
    : IPasskeyAuthenticationVerifier
{
    public async Task<VerifiedPasskeyAuthentication> VerifyAsync(
        string optionsJson,
        string assertionResponseJson,
        CancellationToken cancellationToken)
    {
        var originalOptions =
            AssertionOptions.FromJson(
                optionsJson);

        var assertionResponse =
            JsonSerializer.Deserialize<
                AuthenticatorAssertionRawResponse>(
                    assertionResponseJson)
            ?? throw new InvalidOperationException(
                "Passkey assertion response is invalid.");

        var credential =
            await credentialStore.GetByCredentialIdAsync(
                assertionResponse.RawId,
                cancellationToken)
            ?? throw new InvalidOperationException(
                "Passkey credential is not registered.");

        IsUserHandleOwnerOfCredentialIdAsync
            userHandleOwnsCredential =
                async (args, token) =>
                {
                    var userHandle =
                        await userHandleStore.GetByValueAsync(
                            args.UserHandle,
                            token);

                    return
                        userHandle is not null &&
                        userHandle.UserId == credential.UserId &&
                        credential.CredentialId.SequenceEqual(
                            args.CredentialId);
                };

        var result =
            await fido2.MakeAssertionAsync(
                new MakeAssertionParams
                {
                    AssertionResponse =
                        assertionResponse,

                    OriginalOptions =
                        originalOptions,

                    StoredPublicKey =
                        credential.PublicKey,

                    StoredSignatureCounter =
                        credential.SignCount,

                    IsUserHandleOwnerOfCredentialIdCallback =
                        userHandleOwnsCredential
                },
                cancellationToken);

        return new VerifiedPasskeyAuthentication(
            credential.UserId,
            result.CredentialId,
            result.SignCount);
    }
}
