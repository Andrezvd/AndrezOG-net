namespace AndrezOG.Infrastructure.ContextDb.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AndrezOG.Domain.Model;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> entity)
    {
        entity.ToTable("projects");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();
        entity.Property(e => e.Description).HasMaxLength(500);
        entity.Property(e => e.StartDate).IsRequired();
        entity.Property(e => e.EndDate).IsRequired();
        entity.Property(e => e.IsActive).IsRequired();
        entity.Property(e => e.ImageUrl).HasMaxLength(200);
        entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
        entity.HasIndex(e => e.Title).IsUnique();
        entity.Property(e => e.Type).IsRequired();
        entity.Property(e => e.RepositoryUrl).HasMaxLength(200);
        entity.HasMany(e => e.StackProjects)
              .WithOne(sp => sp.Project)
              .HasForeignKey(sp => sp.IdProject)
              .OnDelete(DeleteBehavior.Cascade);
    }
}