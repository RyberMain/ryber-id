using RyberID.Identity.Application.Users;
using RyberID.Identity.Domain.Users;

namespace RyberID.Identity.Infrastructure.Persistence.Users;

internal sealed class UserStore(
    IdentityDbContext dbContext)
    : IUserStore
{
    public async Task AddAsync(
        User user,
        CancellationToken cancellationToken)
    {
        dbContext.Users.Add(user);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}