using RyberID.Identity.Domain.Passkeys;

namespace RyberID.Identity.Application.Passkeys;

public interface IPasskeyUserHandleStore
{
    Task<PasskeyUserHandle?> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<PasskeyUserHandle?> GetByValueAsync(
        byte[] value,
        CancellationToken cancellationToken);

    Task AddAsync(
        PasskeyUserHandle handle,
        CancellationToken cancellationToken);
}