using Microsoft.Extensions.Caching.Distributed;
using RyberID.Identity.Application.Passkeys;

namespace RyberID.Identity.Infrastructure.Passkeys;

internal sealed class DistributedPasskeyAuthenticationStateStore(
    IDistributedCache cache)
    : IPasskeyAuthenticationStateStore
{
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

    public Task<string?> GetAsync(
        Guid ceremonyId,
        CancellationToken cancellationToken)
    {
        return cache.GetStringAsync(
            GetKey(ceremonyId),
            cancellationToken);
    }

    public Task RemoveAsync(
        Guid ceremonyId,
        CancellationToken cancellationToken)
    {
        return cache.RemoveAsync(
            GetKey(ceremonyId),
            cancellationToken);
    }

    private static string GetKey(Guid ceremonyId)
    {
        return $"passkeys:authentication:{ceremonyId:N}";
    }
}
