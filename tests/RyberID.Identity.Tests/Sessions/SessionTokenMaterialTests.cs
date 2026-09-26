using RyberID.Identity.Application.Sessions;
using RyberID.Identity.Domain.Sessions;

namespace RyberID.Identity.Tests.Sessions;

public sealed class SessionTokenMaterialTests
{
    [Fact]
    public void Constructor_DefensivelyCopiesTokenHash()
    {
        var suppliedHash = CreateHash();
        var expectedHash = suppliedHash.ToArray();
        var material = new SessionTokenMaterial(
            "opaque-token",
            suppliedHash);

        suppliedHash[0] ^= byte.MaxValue;
        var exposedHash = material.Hash;
        exposedHash[1] ^= byte.MaxValue;

        Assert.Equal("opaque-token", material.Value);
        Assert.Equal(expectedHash, material.Hash);
    }

    [Fact]
    public void Constructor_WithInvalidHashLength_IsRejected()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new SessionTokenMaterial(
                "opaque-token",
                new byte[Session.TokenHashSizeInBytes - 1]));

        Assert.Equal("hash", exception.ParamName);
    }

    private static byte[] CreateHash()
    {
        return Enumerable
            .Range(1, Session.TokenHashSizeInBytes)
            .Select(value => (byte)value)
            .ToArray();
    }
}
