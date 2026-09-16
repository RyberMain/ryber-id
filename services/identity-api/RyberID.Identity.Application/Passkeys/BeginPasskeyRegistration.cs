using RyberID.Identity.Domain.Passkeys;
 
namespace RyberID.Identity.Application.Passkeys;

public sealed class BeginPasskeyRegistration(
    IPasskeyUserHandleStore userHandleStore,
    IPasskeyCredentialStore credentialStore,
    IPasskeyRegistrationOptionsFactory optionsFactory)
{
    public async Task<string> ExecuteAsync(
        Guid userId,
        string userName,
        string displayName,
        CancellationToken cancellationToken = default)
    {
        var userHandle =
            await userHandleStore.GetByUserIdAsync(
                userId,
                cancellationToken);

        if (userHandle is null)
        {
            userHandle = PasskeyUserHandle.Create(userId);

            await userHandleStore.AddAsync(
                userHandle,
                cancellationToken);
        }

        var credentials =
            await credentialStore.GetByUserIdAsync(
                userId,
                cancellationToken);

        var existingCredentialIds =
            credentials
                .Select(credential => credential.CredentialId)
                .ToArray();

        return optionsFactory.Create(
            userHandle.Value,
            userName,
            displayName,
            existingCredentialIds);
    }
}
