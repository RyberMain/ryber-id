using System.Security.Cryptography;
using System.Text;
using RyberID.Identity.Application.Sessions;
using RyberID.Identity.Domain.Sessions;

namespace RyberID.Identity.Tests.Support;

internal sealed class InMemorySessionStore
    : ISessionStore
{
    private readonly Dictionary<Guid, Session> _sessions;

    public InMemorySessionStore(
        params Session[] sessions)
    {
        _sessions = sessions.ToDictionary(
            session => session.Id);
    }

    public IReadOnlyCollection<Session> Sessions =>
        _sessions.Values;

    public int TokenHashLookupCount { get; private set; }

    public Task AddAsync(
        Session session,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _sessions.Add(
            session.Id,
            session);

        return Task.CompletedTask;
    }

    public Task<Session?> GetByIdAsync(
        Guid sessionId,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _sessions.TryGetValue(
            sessionId,
            out var session);

        return Task.FromResult(session);
    }

    public Task<Session?> GetByTokenHashAsync(
        byte[] tokenHash,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        TokenHashLookupCount++;

        var session = _sessions.Values.SingleOrDefault(candidate =>
            CryptographicOperations.FixedTimeEquals(
                candidate.TokenHash,
                tokenHash));

        return Task.FromResult(session);
    }

    public Task UpdateAsync(
        Session session,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _sessions[session.Id] = session;

        return Task.CompletedTask;
    }
}

internal sealed class StubSessionTokenService(
    string issuedToken,
    byte[] issuedTokenHash)
    : ISessionTokenService
{
    private readonly List<string> _hashedTokens = [];

    public IReadOnlyList<string> HashedTokens =>
        _hashedTokens;

    public SessionTokenMaterial Issue()
    {
        return new SessionTokenMaterial(
            issuedToken,
            issuedTokenHash);
    }

    public byte[] Hash(
        string token)
    {
        _hashedTokens.Add(token);

        return SessionTestData.HashToken(token);
    }
}

internal sealed class StubSessionLifetimePolicy(
    TimeSpan lifetime)
    : ISessionLifetimePolicy
{
    public TimeSpan GetLifetime()
    {
        return lifetime;
    }
}

internal sealed class StubTimeProvider(
    DateTimeOffset utcNow)
    : TimeProvider
{
    public override DateTimeOffset GetUtcNow()
    {
        return utcNow;
    }
}

internal static class SessionTestData
{
    public static byte[] HashToken(
        string token)
    {
        return SHA256.HashData(
            Encoding.UTF8.GetBytes(token));
    }
}
