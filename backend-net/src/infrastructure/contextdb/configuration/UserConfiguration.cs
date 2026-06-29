namespace AndrezOG.Infrastructure.ContextDb.Configuration;

using Microsoft.EntityFrameworkCore;
using AndrezOG.Domain.Model.Tenant;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.ToTable("users");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd().HasColumnName("id");
        entity.Property(e => e.Email).IsRequired().HasMaxLength(100).HasColumnName("email");
        entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(200).HasColumnName("password_hash");
        entity.Property(e => e.Role).IsRequired().HasColumnName("role");
        entity.Property(e => e.IsActive).IsRequired().HasColumnName("is_active");
        entity.Property(e => e.CreatedAt).IsRequired().HasColumnName("created_at");
        entity.Property(e => e.UpdatedAt).IsRequired().HasColumnName("updated_at");
        entity.Property(e => e.ExternalId).HasColumnName("external_id");
        entity.Property(e => e.Provider).IsRequired().HasColumnName("provider");
        entity.Property(e => e.PhoneNumber).HasColumnName("phone_number");
        entity.Property(e => e.TwoFactorEnabled).IsRequired().HasColumnName("two_factor_enabled");
        entity.Property(e => e.TwoFactorSecret).HasColumnName("two_factor_secret");
        entity.Property(e => e.RefreshToken).HasColumnName("refresh_token");
        entity.Property(e => e.RefreshTokenExpires).HasColumnName("refresh_token_expires");
        entity.Property(e => e.FailedAttempts).IsRequired().HasColumnName("failed_attempts");
        entity.Property(e => e.LockoutEnd).HasColumnName("lockout_end");
        entity.Property(e => e.EmailVerified).IsRequired().HasColumnName("email_verified");
        entity.Property(e => e.EmailVerificationToken).HasColumnName("email_verification_token");
    }
}
