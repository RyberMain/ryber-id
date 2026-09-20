using RyberID.Identity.Domain.Passkeys;

namespace RyberID.Identity.Application.Passkeys;

public interface IPasskeyCredentialStore
{
    Task AddAsync(
        PasskeyCredential credential,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<PasskeyCredential>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<PasskeyCredential?> GetByCredentialIdAsync(
        byte[] credentialId,
        CancellationToken cancellationToken);

    Task<bool> ExistsByCredentialIdAsync(
        byte[] credentialId,
        CancellationToken cancellationToken);

    Task<bool> TryUpdateSignCountAsync(
        Guid credentialRecordId,
        uint expectedSignCount,
        uint newSignCount,
        CancellationToken cancellationToken);
}
