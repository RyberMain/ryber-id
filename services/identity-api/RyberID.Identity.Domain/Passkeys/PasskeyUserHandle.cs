using System.Security.Cryptography;

namespace RyberID.Identity.Domain.Passkeys;

public sealed class PasskeyUserHandle
{
    private const int HandleSize = 64;

    private PasskeyUserHandle()
    {
    }

    private PasskeyUserHandle(
        Guid userId,
        byte[] value)
    {
        UserId = userId;
        Value = value;
    }

    public Guid UserId { get; private set; }

    public byte[] Value { get; private set; } = [];

    public static PasskeyUserHandle Create(Guid userId)
    {
        return new PasskeyUserHandle(
            userId,
            RandomNumberGenerator.GetBytes(HandleSize));
    }
}