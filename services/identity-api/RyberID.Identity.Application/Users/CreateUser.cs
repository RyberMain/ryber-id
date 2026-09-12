using RyberID.Identity.Domain.Users;

namespace RyberID.Identity.Application.Users;

public sealed class CreateUser(IUserStore userStore)
{
    public async Task<Guid> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        var user = User.Create();

        await userStore.AddAsync(user, cancellationToken);

        return user.Id;
    }
}