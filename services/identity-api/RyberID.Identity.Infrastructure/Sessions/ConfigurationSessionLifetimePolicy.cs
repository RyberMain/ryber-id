using System.Globalization;
using Microsoft.Extensions.Configuration;
using RyberID.Identity.Application.Sessions;

namespace RyberID.Identity.Infrastructure.Sessions;

internal sealed class ConfigurationSessionLifetimePolicy
    : ISessionLifetimePolicy
{
    private const string ConfigurationKey =
        "Sessions:Lifetime";

    private readonly TimeSpan _lifetime;

    public ConfigurationSessionLifetimePolicy(
        IConfiguration configuration)
    {
        var configuredValue =
            configuration[ConfigurationKey];

        if (string.IsNullOrWhiteSpace(configuredValue))
        {
            throw new InvalidOperationException(
                $"{ConfigurationKey} was not configured.");
        }

        if (!TimeSpan.TryParse(
                configuredValue,
                CultureInfo.InvariantCulture,
                out var lifetime))
        {
            throw new InvalidOperationException(
                $"{ConfigurationKey} contains an invalid TimeSpan value.");
        }

        if (lifetime <= TimeSpan.Zero)
        {
            throw new InvalidOperationException(
                $"{ConfigurationKey} must be greater than zero.");
        }

        _lifetime = lifetime;
    }

    public TimeSpan GetLifetime()
    {
        return _lifetime;
    }
}
