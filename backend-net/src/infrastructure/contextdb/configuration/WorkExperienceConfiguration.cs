namespace AndrezOG.Infrastructure.ContextDb.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AndrezOG.Domain.Model.Room;

public class WorkExperienceConfiguration : IEntityTypeConfiguration<WorkExperience>
{
    public void Configure(EntityTypeBuilder<WorkExperience> entity)
    {
        entity.ToTable("work_experiences");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();
        entity.Property(e => e.Company).IsRequired().HasMaxLength(100);
        entity.Property(e => e.Position).IsRequired().HasMaxLength(100);
        entity.Property(e => e.StartDate).IsRequired();
        entity.Property(e => e.EndDate);
        entity.Property(e => e.Description).HasMaxLength(500);
        entity.Property(e => e.CompanyUrl).HasMaxLength(200);
        entity.Property(e => e.CompanyLogoUrl).HasMaxLength(200);
        entity.Property(e => e.Order).IsRequired();
        entity.Property(e => e.IsActive).IsRequired();
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
    }
}