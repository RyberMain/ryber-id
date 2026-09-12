namespace RyberID.Identity.Domain.Users;

public sealed class User
{
    private User()
    {
    }

    private User(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; private set; }

    public static User Create()
    {
        return new User(Guid.CreateVersion7());
    }
}