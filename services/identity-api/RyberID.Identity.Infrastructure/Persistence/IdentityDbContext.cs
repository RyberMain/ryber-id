using Microsoft.EntityFrameworkCore;
using RyberID.Identity.Domain.Passkeys;
using RyberID.Identity.Domain.Sessions;
using RyberID.Identity.Domain.Users;

namespace RyberID.Identity.Infrastructure.Persistence;

public sealed class IdentityDbContext(
    DbContextOptions<IdentityDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public DbSet<PasskeyCredential> PasskeyCredentials =>
        Set<PasskeyCredential>();

    public DbSet<PasskeyUserHandle> PasskeyUserHandles =>
    Set<PasskeyUserHandle>();

    public DbSet<Session> Sessions =>
        Set<Session>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(IdentityDbContext).Assembly);
    }
}
