using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RyberID.Identity.Domain.Sessions;

namespace RyberID.Identity.Infrastructure.Persistence.Configurations;

internal sealed class SessionConfiguration
    : IEntityTypeConfiguration<Session>
{
    public void Configure(
        EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("sessions");

        builder.HasKey(session => session.Id);

        builder.Property(session => session.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(session => session.UserId)
            .HasColumnName("user_id");

        builder.Property(session => session.CreatedAtUtc)
            .HasColumnName("created_at_utc");

        builder.Property(session => session.ExpiresAtUtc)
            .HasColumnName("expires_at_utc");

        builder.Property(session => session.RevokedAtUtc)
            .HasColumnName("revoked_at_utc");
    }
}
