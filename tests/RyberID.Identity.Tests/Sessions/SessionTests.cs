using RyberID.Identity.Domain.Sessions;

namespace RyberID.Identity.Tests.Sessions;

public sealed class SessionTests
{
    private static readonly DateTimeOffset CreatedAtUtc =
        new(2026, 9, 20, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_WithValidValues_PreservesItsInvariants()
    {
        var userId = Guid.NewGuid();
        var tokenHash = CreateTokenHash();
        var expectedHash = tokenHash.ToArray();
        var expiresAtUtc = CreatedAtUtc.AddHours(1);

        var session = Session.Create(
            userId,
            tokenHash,
            CreatedAtUtc,
            expiresAtUtc);

        tokenHash[0] ^= byte.MaxValue;
        var exposedHash = session.TokenHash;
        exposedHash[1] ^= byte.MaxValue;

        Assert.NotEqual(Guid.Empty, session.Id);
        Assert.Equal(userId, session.UserId);
        Assert.Equal(expectedHash, session.TokenHash);
        Assert.Equal(CreatedAtUtc, session.CreatedAtUtc);
        Assert.Equal(expiresAtUtc, session.ExpiresAtUtc);
        Assert.Null(session.RevokedAtUtc);
        Assert.True(session.IsActive(CreatedAtUtc));
    }

    [Fact]
    public void Create_WithEmptyUserId_IsRejected()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            Session.Create(
                Guid.Empty,
                CreateTokenHash(),
                CreatedAtUtc,
                CreatedAtUtc.AddHours(1)));

        Assert.Equal("userId", exception.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(Session.TokenHashSizeInBytes - 1)]
    [InlineData(Session.TokenHashSizeInBytes + 1)]
    public void Create_WithInvalidTokenHashLength_IsRejected(
        int length)
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            Session.Create(
                Guid.NewGuid(),
                new byte[length],
                CreatedAtUtc,
                CreatedAtUtc.AddHours(1)));

        Assert.Equal("tokenHash", exception.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithoutPositiveLifetime_IsRejected(
        int expirationOffsetMinutes)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            Session.Create(
                Guid.NewGuid(),
                CreateTokenHash(),
                CreatedAtUtc,
                CreatedAtUtc.AddMinutes(expirationOffsetMinutes)));

        Assert.Equal("expiresAtUtc", exception.ParamName);
    }

    [Fact]
    public void IsActive_UsesCreationInclusiveAndExpirationExclusiveBoundaries()
    {
        var expiresAtUtc = CreatedAtUtc.AddHours(1);
        var session = CreateSession(expiresAtUtc);

        Assert.False(session.IsActive(CreatedAtUtc.AddTicks(-1)));
        Assert.True(session.IsActive(CreatedAtUtc));
        Assert.True(session.IsActive(expiresAtUtc.AddTicks(-1)));
        Assert.False(session.IsActive(expiresAtUtc));
    }

    [Fact]
    public void Revoke_MakesSessionInactiveAndIsIdempotent()
    {
        var session = CreateSession(CreatedAtUtc.AddHours(1));
        var revokedAtUtc = CreatedAtUtc.AddMinutes(10);

        session.Revoke(revokedAtUtc);
        session.Revoke(revokedAtUtc.AddMinutes(5));

        Assert.Equal(revokedAtUtc, session.RevokedAtUtc);
        Assert.False(session.IsActive(revokedAtUtc));
    }

    [Fact]
    public void Revoke_BeforeCreation_IsRejected()
    {
        var session = CreateSession(CreatedAtUtc.AddHours(1));

        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            session.Revoke(CreatedAtUtc.AddTicks(-1)));

        Assert.Equal("revokedAtUtc", exception.ParamName);
        Assert.Null(session.RevokedAtUtc);
    }

    private static Session CreateSession(
        DateTimeOffset expiresAtUtc)
    {
        return Session.Create(
            Guid.NewGuid(),
            CreateTokenHash(),
            CreatedAtUtc,
            expiresAtUtc);
    }

    private static byte[] CreateTokenHash()
    {
        return Enumerable
            .Range(0, Session.TokenHashSizeInBytes)
            .Select(value => (byte)value)
            .ToArray();
    }
}
