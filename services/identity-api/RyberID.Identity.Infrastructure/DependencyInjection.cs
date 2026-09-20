using RyberID.Identity.Infrastructure.Sessions;
using RyberID.Identity.Infrastructure.Persistence.Sessions;
using RyberID.Identity.Application.Sessions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RyberID.Identity.Application.Users;
using RyberID.Identity.Infrastructure.Persistence;
using RyberID.Identity.Infrastructure.Persistence.Users;
using RyberID.Identity.Application.Passkeys;
using RyberID.Identity.Infrastructure.Persistence.Passkeys;

using RyberID.Identity.Infrastructure.Passkeys;

namespace RyberID.Identity.Infrastructure;

public static class DependencyInjection
{
    private const string ConnectionStringName = "IdentityDatabase";

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString(ConnectionStringName)
            ?? throw new InvalidOperationException(
                $"Connection string '{ConnectionStringName}' was not configured.");

        services.AddSingleton<TimeProvider>(
            TimeProvider.System);

        services.AddSingleton<ISessionLifetimePolicy>(
            serviceProvider =>
                new ConfigurationSessionLifetimePolicy(
                    serviceProvider.GetRequiredService<IConfiguration>()));
        services.AddDbContext<IdentityDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUserStore, UserStore>();
        services.AddScoped<ISessionStore, SessionStore>();
        services.AddScoped<IPasskeyCredentialStore, PasskeyCredentialStore>();
        services.AddScoped<IPasskeyUserHandleStore, PasskeyUserHandleStore>();
        services.AddScoped<
            IPasskeyRegistrationOptionsFactory,
            Fido2PasskeyRegistrationOptionsFactory>();

        services.AddDistributedMemoryCache();

        services.AddScoped<
            IPasskeyRegistrationStateStore,
            DistributedPasskeyRegistrationStateStore>();

        services.AddScoped<
            IPasskeyRegistrationVerifier,
            Fido2PasskeyRegistrationVerifier>();

        services.AddScoped<
            IPasskeyAuthenticationOptionsFactory,
            Fido2PasskeyAuthenticationOptionsFactory>();

        services.AddScoped<
            IPasskeyAuthenticationStateStore,
            DistributedPasskeyAuthenticationStateStore>();

        services.AddScoped<
            IPasskeyAuthenticationVerifier,
            Fido2PasskeyAuthenticationVerifier>();

        services.AddPasskeyFido2(configuration);

        return services;
    }
}






