namespace AndrezOG.Infrastructure.ContextDb.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AndrezOG.Domain.Model.Skills;

public class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> entity)
    {
        entity.ToTable("skills");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd().HasColumnName("id");
        entity.Property(e => e.Name).IsRequired().HasMaxLength(100).HasColumnName("name");
        entity.Property(e => e.Description).HasMaxLength(500).HasColumnName("description");
        entity.Property(e => e.SkillType).IsRequired().HasColumnName("type_skill");
        entity.Property(e => e.IsActive).IsRequired().HasColumnName("is_active");
        entity.Property(e => e.ImageUrl).IsRequired().HasMaxLength(200).HasColumnName("image_url");
        entity.Property(e => e.CreatedAt).IsRequired().HasColumnName("created_at");
        entity.Property(e => e.UpdatedAt).IsRequired().HasColumnName("updated_at");

        // uniques
        entity.HasIndex(e => e.Name).IsUnique();
    }
}
