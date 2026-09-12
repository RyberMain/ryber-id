using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RyberID.Identity.Infrastructure.Persistence;

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

        services.AddDbContext<IdentityDbContext>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }
}