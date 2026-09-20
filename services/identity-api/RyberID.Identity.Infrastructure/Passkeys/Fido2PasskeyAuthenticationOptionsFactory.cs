using Fido2NetLib;
using Fido2NetLib.Objects;
using RyberID.Identity.Application.Passkeys;

namespace RyberID.Identity.Infrastructure.Passkeys;

internal sealed class Fido2PasskeyAuthenticationOptionsFactory(
    IFido2 fido2)
    : IPasskeyAuthenticationOptionsFactory
{
    public PasskeyAuthenticationOptions Create()
    {
        var options =
            fido2.GetAssertionOptions(
                new GetAssertionOptionsParams
                {
                    AllowedCredentials =
                        Array.Empty<PublicKeyCredentialDescriptor>(),

                    UserVerification =
                        UserVerificationRequirement.Preferred
                });

        return new PasskeyAuthenticationOptions(
            options.ToJson(),
            options.Timeout);
    }
}
