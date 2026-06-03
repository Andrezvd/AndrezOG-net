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
        entity.Property(e => e.Backend).IsRequired().HasMaxLength(100);
        entity.Property(e => e.Frontend).IsRequired().HasMaxLength(100);
        entity.Property(e => e.Database).IsRequired().HasMaxLength(100);
        entity.Property(e => e.Cicd).IsRequired().HasMaxLength(100);
        entity.Property(e => e.Cloud).IsRequired().HasMaxLength(100);
        entity.Property(e => e.Conteinerization).IsRequired().HasMaxLength(100);
        entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
        entity.Property(e => e.IsActive).IsRequired();
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.HasMany(e => e.StackProjects)
              .WithOne(sp => sp.Stack)
              .HasForeignKey(sp => sp.IdStack)
              .OnDelete(DeleteBehavior.Cascade);

    }
}