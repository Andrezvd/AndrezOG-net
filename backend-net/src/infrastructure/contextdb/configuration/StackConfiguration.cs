namespace AndrezOG.Infrastructure.ContextDb.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AndrezOG.Domain.Model.StackProject;

public class StackConfiguration : IEntityTypeConfiguration<Stack>
{
    public void Configure(EntityTypeBuilder<Stack> entity)
    {
        entity.ToTable("stacks");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();
        entity.Property(e => e.Summary).IsRequired().HasMaxLength(500);
        entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
        entity.Property(e => e.IsActive).IsRequired();
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.HasMany(e => e.StackProjects)
              .WithOne(sp => sp.Stack)
              .HasForeignKey(sp => sp.IdStack)
              .OnDelete(DeleteBehavior.Cascade);
        entity.HasMany(e => e.StackSkills)
              .WithOne(ss => ss.Stack)
              .HasForeignKey(ss => ss.IdStack)
              .OnDelete(DeleteBehavior.Cascade);

    }
}