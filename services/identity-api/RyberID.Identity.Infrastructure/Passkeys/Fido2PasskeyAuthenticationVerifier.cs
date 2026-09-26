using System.Text.Json;
using Fido2NetLib;
using Fido2NetLib.Objects;
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
        AssertionOptions originalOptions;
        AuthenticatorAssertionRawResponse assertionResponse;

        try
        {
            originalOptions =
                AssertionOptions.FromJson(
                    optionsJson);

            assertionResponse =
                JsonSerializer.Deserialize<
                    AuthenticatorAssertionRawResponse>(
                        assertionResponseJson)
                ?? throw new PasskeyAuthenticationFailedException();
        }
        catch (Exception exception)
            when (exception is JsonException or
                FormatException or
                Fido2VerificationException)
        {
            throw new PasskeyAuthenticationFailedException(
                exception);
        }

        if (assertionResponse.RawId is not { Length: > 0 } ||
            assertionResponse.Response?.UserHandle is not { Length: > 0 })
        {
            throw new PasskeyAuthenticationFailedException();
        }

        var credential =
            await credentialStore.GetByCredentialIdAsync(
                assertionResponse.RawId,
                cancellationToken)
            ?? throw new PasskeyAuthenticationFailedException();

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

        VerifyAssertionResult result;

        try
        {
            result =
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
        }
        catch (Fido2VerificationException exception)
        {
            throw new PasskeyAuthenticationFailedException(
                exception);
        }

        return new VerifiedPasskeyAuthentication(
            credential.UserId,
            credential.Id,
            credential.SignCount,
            result.SignCount);
    }
}
