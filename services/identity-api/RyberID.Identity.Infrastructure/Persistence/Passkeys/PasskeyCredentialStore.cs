using Microsoft.EntityFrameworkCore;
using RyberID.Identity.Application.Passkeys;
using RyberID.Identity.Domain.Passkeys;

namespace RyberID.Identity.Infrastructure.Persistence.Passkeys;

internal sealed class PasskeyCredentialStore(
    IdentityDbContext dbContext)
    : IPasskeyCredentialStore
{
    public async Task AddAsync(
        PasskeyCredential credential,
        CancellationToken cancellationToken)
    {
        dbContext.PasskeyCredentials.Add(credential);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PasskeyCredential>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await dbContext.PasskeyCredentials
            .AsNoTracking()
            .Where(credential => credential.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public Task<PasskeyCredential?> GetByCredentialIdAsync(
        byte[] credentialId,
        CancellationToken cancellationToken)
    {
        return dbContext.PasskeyCredentials
            .AsNoTracking()
            .SingleOrDefaultAsync(
                credential =>
                    credential.CredentialId.SequenceEqual(credentialId),
                cancellationToken);
    }

    public Task<bool> ExistsByCredentialIdAsync(
        byte[] credentialId,
        CancellationToken cancellationToken)
    {
        return dbContext.PasskeyCredentials
            .AnyAsync(
                credential =>
                    credential.CredentialId.SequenceEqual(credentialId),
                cancellationToken);
    }

    public async Task<bool> TryUpdateSignCountAsync(
        Guid credentialRecordId,
        uint expectedSignCount,
        uint newSignCount,
        CancellationToken cancellationToken)
    {
        var updatedRows =
            await dbContext.PasskeyCredentials
                .Where(
                    credential =>
                        credential.Id == credentialRecordId &&
                        credential.SignCount == expectedSignCount)
                .ExecuteUpdateAsync(
                    setters =>
                        setters.SetProperty(
                            credential => credential.SignCount,
                            newSignCount),
                    cancellationToken);

        return updatedRows == 1;
    }
}
