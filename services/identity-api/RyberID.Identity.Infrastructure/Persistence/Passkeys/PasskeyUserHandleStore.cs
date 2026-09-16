using Microsoft.EntityFrameworkCore;
using RyberID.Identity.Application.Passkeys;
using RyberID.Identity.Domain.Passkeys;

namespace RyberID.Identity.Infrastructure.Persistence.Passkeys;

internal sealed class PasskeyUserHandleStore(
    IdentityDbContext dbContext)
    : IPasskeyUserHandleStore
{
    public Task<PasskeyUserHandle?> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return dbContext.PasskeyUserHandles
            .AsNoTracking()
            .SingleOrDefaultAsync(
                handle => handle.UserId == userId,
                cancellationToken);
    }

    public Task<PasskeyUserHandle?> GetByValueAsync(
        byte[] value,
        CancellationToken cancellationToken)
    {
        return dbContext.PasskeyUserHandles
            .AsNoTracking()
            .SingleOrDefaultAsync(
                handle => handle.Value.SequenceEqual(value),
                cancellationToken);
    }

    public async Task AddAsync(
        PasskeyUserHandle handle,
        CancellationToken cancellationToken)
    {
        dbContext.PasskeyUserHandles.Add(handle);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}