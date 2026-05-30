namespace AndrezOG.Infrastructure.ContextDb.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AndrezOG.Domain.Model;

public class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> entity)
    {
        entity.ToTable("skills");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();
        entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        entity.Property(e => e.Description).HasMaxLength(500);
        entity.Property(e => e.SkillType).IsRequired();
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.ImageUrl).IsRequired().HasMaxLength(200);
    }
}