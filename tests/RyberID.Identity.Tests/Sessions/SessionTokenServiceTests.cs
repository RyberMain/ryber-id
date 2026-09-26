using RyberID.Identity.Application.Sessions;
using RyberID.Identity.Domain.Sessions;
using RyberID.Identity.Infrastructure;

namespace RyberID.Identity.Tests.Sessions;

public sealed class SessionTokenServiceTests
{
    [Fact]
    public void Issue_GeneratesDistinctOpaqueTokensWithMatchingHashes()
    {
        var sut = CreateTokenService();

        var first = sut.Issue();
        var second = sut.Issue();

        Assert.NotEqual(first.Value, second.Value);
        Assert.False(Guid.TryParse(first.Value, out _));
        Assert.False(Guid.TryParse(second.Value, out _));
        Assert.Equal(Session.TokenHashSizeInBytes, first.Hash.Length);
        Assert.Equal(Session.TokenHashSizeInBytes, second.Hash.Length);
        Assert.Equal(first.Hash, sut.Hash(first.Value));
        Assert.Equal(second.Hash, sut.Hash(second.Value));
    }

    [Fact]
    public void Hash_IsDeterministicAndSensitiveToTokenValue()
    {
        var sut = CreateTokenService();

        var firstHash = sut.Hash("first-session-token");
        var repeatedHash = sut.Hash("first-session-token");
        var differentHash = sut.Hash("second-session-token");

        Assert.Equal(firstHash, repeatedHash);
        Assert.NotEqual(firstHash, differentHash);
        Assert.Equal(Session.TokenHashSizeInBytes, firstHash.Length);
    }

    private static ISessionTokenService CreateTokenService()
    {
        const string implementationTypeName =
            "RyberID.Identity.Infrastructure.Sessions.SessionTokenService";

        var implementationType = typeof(DependencyInjection)
            .Assembly
            .GetType(
                implementationTypeName,
                throwOnError: true)!;

        var instance = Activator.CreateInstance(
            implementationType,
            nonPublic: true);

        return Assert.IsAssignableFrom<ISessionTokenService>(
            instance);
    }
}
