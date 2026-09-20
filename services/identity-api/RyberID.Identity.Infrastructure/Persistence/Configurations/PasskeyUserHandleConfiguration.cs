using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RyberID.Identity.Domain.Passkeys;
using RyberID.Identity.Domain.Users;

namespace RyberID.Identity.Infrastructure.Persistence.Configurations;

internal sealed class PasskeyUserHandleConfiguration
    : IEntityTypeConfiguration<PasskeyUserHandle>
{
    public void Configure(
        EntityTypeBuilder<PasskeyUserHandle> builder)
    {
        builder.ToTable("passkey_user_handles");

        builder.HasKey(handle => handle.UserId);

        builder.Property(handle => handle.UserId)
            .HasColumnName("user_id")
            .ValueGeneratedNever();

        builder.Property(handle => handle.Value)
            .HasColumnName("value")
            .HasField("_value")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .IsRequired();

        builder.HasIndex(handle => handle.Value)
            .IsUnique();

        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<PasskeyUserHandle>(
                handle => handle.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
