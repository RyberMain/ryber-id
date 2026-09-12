using RyberID.Identity.Domain.Users;

namespace RyberID.Identity.Application.Users;

public interface IUserStore
{
    Task AddAsync(
        User user,
        CancellationToken cancellationToken);
}