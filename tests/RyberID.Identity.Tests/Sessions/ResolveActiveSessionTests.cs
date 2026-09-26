using RyberID.Identity.Application.Sessions;
using RyberID.Identity.Domain.Sessions;
using RyberID.Identity.Tests.Support;

namespace RyberID.Identity.Tests.Sessions;

public sealed class ResolveActiveSessionTests
{
    private static readonly DateTimeOffset NowUtc =
        new(2026, 9, 20, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task ExecuteAsync_WithActiveSession_ReturnsItsIdentity()
    {
        const string token = "active-session-token";
        var tokenHash = SessionTestData.HashToken(token);
        var session = Session.Create(
            Guid.NewGuid(),
            tokenHash,
            NowUtc.AddMinutes(-5),
            NowUtc.AddHours(1));
        var store = new InMemorySessionStore(session);
        var tokenService = new StubSessionTokenService(
            "unused-issued-token",
            SessionTestData.HashToken("unused-issued-token"));
        var sut = new ResolveActiveSession(
            store,
            tokenService,
            new StubTimeProvider(NowUtc));

        var result = await sut.ExecuteAsync(token);

        Assert.NotNull(result);
        Assert.Equal(session.Id, result.SessionId);
        Assert.Equal(session.UserId, result.UserId);
        Assert.Equal(token, Assert.Single(tokenService.HashedTokens));
    }

    [Fact]
    public async Task ExecuteAsync_WithRevokedSession_ReturnsNull()
    {
        const string token = "revoked-session-token";
        var session = CreateSession(
            token,
            NowUtc.AddMinutes(-5),
            NowUtc.AddHours(1));
        session.Revoke(NowUtc.AddMinutes(-1));
        var sut = CreateResolver(session);

        var result = await sut.ExecuteAsync(token);

        Assert.Null(result);
    }

    [Fact]
    public async Task ExecuteAsync_WithExpiredSession_ReturnsNull()
    {
        const string token = "expired-session-token";
        var session = CreateSession(
            token,
            NowUtc.AddHours(-1),
            NowUtc);
        var sut = CreateResolver(session);

        var result = await sut.ExecuteAsync(token);

        Assert.Null(result);
    }

    [Fact]
    public async Task ExecuteAsync_WithSessionThatHasNotStarted_ReturnsNull()
    {
        const string token = "future-session-token";
        var session = CreateSession(
            token,
            NowUtc.AddMinutes(1),
            NowUtc.AddHours(1));
        var sut = CreateResolver(session);

        var result = await sut.ExecuteAsync(token);

        Assert.Null(result);
    }

    [Fact]
    public async Task ExecuteAsync_WithUnknownToken_ReturnsNull()
    {
        var sut = CreateResolver();

        var result = await sut.ExecuteAsync(
            "unknown-session-token");

        Assert.Null(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ExecuteAsync_WithoutToken_DoesNotHashOrQueryStore(
        string token)
    {
        var store = new InMemorySessionStore();
        var tokenService = new StubSessionTokenService(
            "unused-issued-token",
            SessionTestData.HashToken("unused-issued-token"));
        var sut = new ResolveActiveSession(
            store,
            tokenService,
            new StubTimeProvider(NowUtc));

        var result = await sut.ExecuteAsync(token);

        Assert.Null(result);
        Assert.Empty(tokenService.HashedTokens);
        Assert.Equal(0, store.TokenHashLookupCount);
    }

    private static ResolveActiveSession CreateResolver(
        params Session[] sessions)
    {
        return new ResolveActiveSession(
            new InMemorySessionStore(sessions),
            new StubSessionTokenService(
                "unused-issued-token",
                SessionTestData.HashToken("unused-issued-token")),
            new StubTimeProvider(NowUtc));
    }

    private static Session CreateSession(
        string token,
        DateTimeOffset createdAtUtc,
        DateTimeOffset expiresAtUtc)
    {
        return Session.Create(
            Guid.NewGuid(),
            SessionTestData.HashToken(token),
            createdAtUtc,
            expiresAtUtc);
    }
}
