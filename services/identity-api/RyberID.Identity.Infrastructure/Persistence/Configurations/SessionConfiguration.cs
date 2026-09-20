using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RyberID.Identity.Domain.Sessions;
using RyberID.Identity.Domain.Users;

namespace RyberID.Identity.Infrastructure.Persistence.Configurations;

internal sealed class SessionConfiguration
    : IEntityTypeConfiguration<Session>
{
    public void Configure(
        EntityTypeBuilder<Session> builder)
    {
        builder.ToTable(
            "sessions",
            tableBuilder =>
                tableBuilder.HasCheckConstraint(
                    "CK_sessions_token_hash_length",
                    "octet_length(token_hash) = 32"));

        builder.HasKey(
            session => session.Id);

        builder.Property(
                session => session.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(
                session => session.UserId)
            .HasColumnName("user_id");

        builder.Property(
                session => session.TokenHash)
            .HasColumnName("token_hash")
            .HasField("_tokenHash")
            .UsePropertyAccessMode(
                PropertyAccessMode.Field)
            .IsRequired();

        builder.Property(
                session => session.CreatedAtUtc)
            .HasColumnName("created_at_utc");

        builder.Property(
                session => session.ExpiresAtUtc)
            .HasColumnName("expires_at_utc");

        builder.Property(
                session => session.RevokedAtUtc)
            .HasColumnName("revoked_at_utc");

        builder.HasIndex(
                session => session.TokenHash)
            .IsUnique();

        builder.HasIndex(
            session => session.UserId);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(
                session => session.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
