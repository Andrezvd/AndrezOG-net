namespace AndrezOG.Infrastructure.ContextDb.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AndrezOG.Domain.Model.StackProject;

public class StackSkillConfiguration : IEntityTypeConfiguration<StackSkill>
{
    public void Configure(EntityTypeBuilder<StackSkill> entity)
    {
        entity.ToTable("stack_skills");
        entity.HasKey(e => new { e.IdStack, e.IdSkill });
        entity.Property(e => e.StackRole)
              .IsRequired()
              .HasConversion<string>()
              .HasMaxLength(50);
        entity.HasOne(e => e.Stack)
              .WithMany(s => s.StackSkills)
              .HasForeignKey(e => e.IdStack)
              .OnDelete(DeleteBehavior.Cascade);
        entity.HasOne(e => e.Skill)
              .WithMany()
              .HasForeignKey(e => e.IdSkill)
              .OnDelete(DeleteBehavior.Cascade);
    }
}