using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RyberID.Identity.Infrastructure.Persistence;

public sealed class IdentityDbContextFactory
    : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(
        string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable(
                "ConnectionStrings__IdentityDatabase")
            ?? throw new InvalidOperationException(
                "ConnectionStrings__IdentityDatabase was not configured for EF Core design-time operations.");

        var options =
            new DbContextOptionsBuilder<IdentityDbContext>()
                .UseNpgsql(connectionString)
                .Options;

        return new IdentityDbContext(options);
    }
}
