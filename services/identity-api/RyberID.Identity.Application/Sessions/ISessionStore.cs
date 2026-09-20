using RyberID.Identity.Domain.Sessions;

namespace RyberID.Identity.Application.Sessions;

public interface ISessionStore
{
    Task AddAsync(
        Session session,
        CancellationToken cancellationToken);

    Task<Session?> GetByIdAsync(
        Guid sessionId,
        CancellationToken cancellationToken);

    Task<Session?> GetByTokenHashAsync(
        byte[] tokenHash,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        Session session,
        CancellationToken cancellationToken);
}
