using RyberID.Identity.Application.Authentication;
using RyberID.Identity.Application.Sessions;
using RyberID.Identity.Domain.Sessions;
using RyberID.Identity.Tests.Support;

namespace RyberID.Identity.Tests.Sessions;

public sealed class CreateSessionTests
{
    [Fact]
    public async Task ExecuteAsync_UsesServerPolicyAndPersistsOnlyTokenHash()
    {
        var nowUtc = new DateTimeOffset(
            2026,
            9,
            20,
            12,
            0,
            0,
            TimeSpan.Zero);
        var lifetime = TimeSpan.FromHours(6);
        var token = "opaque-session-token";
        var tokenHash = SessionTestData.HashToken(token);
        var store = new InMemorySessionStore();
        var sut = new CreateSession(
            store,
            new StubSessionTokenService(token, tokenHash),
            new StubSessionLifetimePolicy(lifetime),
            new StubTimeProvider(nowUtc));
        var identity = new AuthenticatedIdentity(
            Guid.NewGuid());

        var result = await sut.ExecuteAsync(identity);

        var persisted = Assert.Single(store.Sessions);
        Assert.Equal(token, result.Token);
        Assert.NotEqual(persisted.Id.ToString(), result.Token);
        Assert.Equal(tokenHash, persisted.TokenHash);
        Assert.Equal(persisted.Id, result.Session.SessionId);
        Assert.Equal(identity.UserId, persisted.UserId);
        Assert.Equal(nowUtc, persisted.CreatedAtUtc);
        Assert.Equal(nowUtc.Add(lifetime), persisted.ExpiresAtUtc);
        Assert.True(result.Session.IsActive);
    }
}
