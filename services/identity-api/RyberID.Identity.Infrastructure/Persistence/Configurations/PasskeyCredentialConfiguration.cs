using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RyberID.Identity.Domain.Passkeys;

namespace RyberID.Identity.Infrastructure.Persistence.Configurations;

internal sealed class PasskeyCredentialConfiguration
    : IEntityTypeConfiguration<PasskeyCredential>
{
    public void Configure(
        EntityTypeBuilder<PasskeyCredential> builder)
    {
        builder.ToTable("passkey_credentials");

        builder.HasKey(passkey => passkey.Id);

        builder.Property(passkey => passkey.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(passkey => passkey.UserId)
            .HasColumnName("user_id");

        builder.Property(passkey => passkey.CredentialId)
            .HasColumnName("credential_id")
            .HasField("_credentialId")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .IsRequired();

        builder.Property(passkey => passkey.PublicKey)
            .HasColumnName("public_key")
            .HasField("_publicKey")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .IsRequired();

        builder.Property(passkey => passkey.SignCount)
            .HasColumnName("sign_count");

        builder.HasIndex(passkey => passkey.CredentialId)
            .IsUnique();

        builder.HasOne<RyberID.Identity.Domain.Users.User>()
            .WithMany()
            .HasForeignKey(passkey => passkey.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
