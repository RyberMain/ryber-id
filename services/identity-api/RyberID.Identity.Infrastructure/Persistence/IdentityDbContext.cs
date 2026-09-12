using Microsoft.EntityFrameworkCore;
using RyberID.Identity.Domain.Users;

namespace RyberID.Identity.Infrastructure.Persistence;

public sealed class IdentityDbContext(
    DbContextOptions<IdentityDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(IdentityDbContext).Assembly);
    }
}