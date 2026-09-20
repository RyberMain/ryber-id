using RyberID.Identity.Domain.Passkeys;

namespace RyberID.Identity.Application.Passkeys;

public sealed class BeginPasskeyRegistration(
    IPasskeyUserHandleStore userHandleStore,
    IPasskeyCredentialStore credentialStore,
    IPasskeyRegistrationOptionsFactory optionsFactory,
    IPasskeyRegistrationStateStore stateStore)
{
    public async Task<PasskeyRegistrationStart> ExecuteAsync(
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

        var options =
            optionsFactory.Create(
                userHandle.Value,
                userName,
                displayName,
                existingCredentialIds);

        var ceremonyId = Guid.CreateVersion7();

        var lifetime =
            TimeSpan.FromMilliseconds(
                (double)options.TimeoutMilliseconds);

        await stateStore.SaveAsync(
            ceremonyId,
            options.Json,
            lifetime,
            cancellationToken);

        return new PasskeyRegistrationStart(
            ceremonyId,
            options.Json);
    }
}
