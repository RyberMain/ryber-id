using Microsoft.EntityFrameworkCore;
using RyberID.Identity.Application.Sessions;
using RyberID.Identity.Domain.Sessions;

namespace RyberID.Identity.Infrastructure.Persistence.Sessions;

internal sealed class SessionStore(
    IdentityDbContext dbContext)
    : ISessionStore
{
    public async Task AddAsync(
        Session session,
        CancellationToken cancellationToken)
    {
        dbContext.Sessions.Add(session);

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public Task<Session?> GetByIdAsync(
        Guid sessionId,
        CancellationToken cancellationToken)
    {
        return dbContext.Sessions
            .AsNoTracking()
            .SingleOrDefaultAsync(
                session => session.Id == sessionId,
                cancellationToken);
    }

    public async Task UpdateAsync(
        Session session,
        CancellationToken cancellationToken)
    {
        dbContext.Sessions.Update(session);

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }
}
