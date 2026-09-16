using Fido2NetLib;
using Fido2NetLib.Objects;
using RyberID.Identity.Application.Passkeys;

namespace RyberID.Identity.Infrastructure.Passkeys;

internal sealed class Fido2PasskeyRegistrationOptionsFactory(
    IFido2 fido2)
    : IPasskeyRegistrationOptionsFactory
{
    public string Create(
        byte[] userHandle,
        string userName,
        string displayName,
        IReadOnlyCollection<byte[]> existingCredentialIds)
    {
        var user = new Fido2User
        {
            Id = userHandle,
            Name = userName,
            DisplayName = displayName
        };

        var excludedCredentials =
            existingCredentialIds
                .Select(
                    credentialId =>
                        new PublicKeyCredentialDescriptor(
                            credentialId))
                .ToArray();

        var options =
            fido2.RequestNewCredential(
                new RequestNewCredentialParams
                {
                    User = user,
                    ExcludeCredentials = excludedCredentials,
                    AuthenticatorSelection =
                        new AuthenticatorSelection
                        {
                            ResidentKey =
                                ResidentKeyRequirement.Required,
                            UserVerification =
                                UserVerificationRequirement.Preferred
                        },
                    AttestationPreference =
                        AttestationConveyancePreference.None
                });

        return options.ToJson();
    }
}
