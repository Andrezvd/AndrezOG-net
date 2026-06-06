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
        entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
        entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
        entity.Property(e => e.Summary).HasMaxLength(500);
        entity.Property(e => e.Education).HasMaxLength(200);
        entity.Property(e => e.EducationStartYear).HasMaxLength(4);
        entity.Property(e => e.EducationEndYear).HasMaxLength(4);
        entity.Property(e => e.PhotoUrl).HasMaxLength(200);
        entity.Property(e => e.VideoUrl).HasMaxLength(200);
        entity.Property(e => e.Email).HasMaxLength(100);
        entity.Property(e => e.LinkedInUrl).HasMaxLength(200);
        entity.Property(e => e.GitHubUrl).HasMaxLength(200); 
    }
}