using System.Security.Cryptography;

namespace RyberID.Identity.Domain.Passkeys;

public sealed class PasskeyUserHandle
{
    private const int HandleSize = 64;

    private byte[] _value = [];

    private PasskeyUserHandle()
    {
    }

    private PasskeyUserHandle(
        Guid userId,
        byte[] value)
    {
        UserId = userId;
        _value = value.ToArray();
    }

    public Guid UserId { get; private set; }

    public byte[] Value => _value.ToArray();

    public static PasskeyUserHandle Create(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException(
                "Passkey user identifier cannot be empty.",
                nameof(userId));
        }

        return new PasskeyUserHandle(
            userId,
            RandomNumberGenerator.GetBytes(HandleSize));
    }
}
