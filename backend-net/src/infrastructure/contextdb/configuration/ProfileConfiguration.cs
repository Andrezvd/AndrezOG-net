namespace AndrezOG.Infrastructure.ContextDb.Configuration;

using Microsoft.EntityFrameworkCore;
using AndrezOG.Domain.Model.Tenant;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> entity)
    {
        entity.ToTable("profiles");
        entity.HasKey(e => e.IdUser);
        entity.Property(e => e.IdUser).ValueGeneratedOnAdd().HasColumnName("id_user");
        entity.Property(e => e.Name).IsRequired().HasMaxLength(100).HasColumnName("name");
        entity.Property(e => e.LastName).IsRequired().HasMaxLength(100).HasColumnName("last_name");
        entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(20).HasColumnName("phone_number");
        entity.Property(e => e.Country).IsRequired().HasMaxLength(50).HasColumnName("country");
        entity.Property(e => e.City).IsRequired().HasMaxLength(50).HasColumnName("city");
        entity.Property(e => e.State).IsRequired().HasMaxLength(50).HasColumnName("state");
        entity.Property(e => e.ZipCode).IsRequired().HasMaxLength(10).HasColumnName("zip_code");
        entity.Property(e => e.Title).IsRequired().HasMaxLength(200).HasColumnName("title");
        entity.Property(e => e.Summary).HasMaxLength(500).HasColumnName("summary");
        entity.Property(e => e.Available).IsRequired().HasColumnName("available");
        entity.Property(e => e.AvailableText).HasColumnName("available_text");
        entity.Property(e => e.Education).HasMaxLength(200).HasColumnName("education");
        entity.Property(e => e.EducationStartYear).HasColumnName("education_start_year");
        entity.Property(e => e.EducationEndYear).HasColumnName("education_end_year");
        entity.Property(e => e.PhotoUrl).HasMaxLength(500).HasColumnName("photo_url");
        entity.Property(e => e.VideoUrl).HasMaxLength(500).HasColumnName("video_url");
        entity.Property(e => e.Email).HasMaxLength(255).HasColumnName("email");
        entity.Property(e => e.LinkedInUrl).HasMaxLength(500).HasColumnName("linked_in_url");
        entity.Property(e => e.GitHubUrl).HasMaxLength(500).HasColumnName("git_hub_url");
        entity.Property(e => e.CreatedAt).IsRequired().HasColumnName("created_at");
        entity.Property(e => e.UpdatedAt).IsRequired().HasColumnName("updated_at");
    }
}