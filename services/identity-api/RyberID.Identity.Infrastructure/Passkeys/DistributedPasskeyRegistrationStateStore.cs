using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Distributed;
using RyberID.Identity.Application.Passkeys;

namespace RyberID.Identity.Infrastructure.Passkeys;

internal sealed class DistributedPasskeyRegistrationStateStore(
    IDistributedCache cache)
    : IPasskeyRegistrationStateStore
{
    private static readonly ConcurrentDictionary<Guid, SemaphoreSlim>
        CeremonyLocks = new();

    public Task SaveAsync(
        Guid ceremonyId,
        string optionsJson,
        TimeSpan lifetime,
        CancellationToken cancellationToken)
    {
        var options =
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = lifetime
            };

        return cache.SetStringAsync(
            GetKey(ceremonyId),
            optionsJson,
            options,
            cancellationToken);
    }

    public async Task<string?> ConsumeAsync(
        Guid ceremonyId,
        CancellationToken cancellationToken)
    {
        var semaphore =
            CeremonyLocks.GetOrAdd(
                ceremonyId,
                static _ => new SemaphoreSlim(1, 1));

        await semaphore.WaitAsync(cancellationToken);

        try
        {
            var key = GetKey(ceremonyId);

            var value =
                await cache.GetStringAsync(
                    key,
                    cancellationToken);

            if (value is null)
            {
                return null;
            }

            await cache.RemoveAsync(
                key,
                cancellationToken);

            return value;
        }
        finally
        {
            semaphore.Release();

            CeremonyLocks.TryRemove(
                ceremonyId,
                out _);
        }
    }

    private static string GetKey(Guid ceremonyId)
    {
        return $"passkeys:registration:{ceremonyId:N}";
    }
}
