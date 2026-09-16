using Fido2NetLib;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RyberID.Identity.Infrastructure.Passkeys;

internal static class PasskeyFido2Registration
{
    public static IServiceCollection AddPasskeyFido2(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var serverDomain =
            configuration["Passkeys:ServerDomain"]
            ?? throw new InvalidOperationException(
                "Passkeys:ServerDomain was not configured.");

        var origins = configuration
            .GetSection("Passkeys:Origins")
            .GetChildren()
            .Select(origin => origin.Value)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value!)
            .ToHashSet(StringComparer.Ordinal);

        if (origins.Count == 0)
        {
            throw new InvalidOperationException(
                "At least one Passkeys:Origins value must be configured.");
        }

        services.AddFido2(options =>
        {
            options.ServerDomain = serverDomain;
            options.ServerName = "RyberID";
            options.Origins = origins;
        });

        return services;
    }
}
