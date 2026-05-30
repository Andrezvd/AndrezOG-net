namespace AndrezOG.Infrastructure.ContextDb.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AndrezOG.Domain.Model;

public class StackProjectConfiguration : IEntityTypeConfiguration<StackProject>
{
    public void Configure(EntityTypeBuilder<StackProject> entity)
    {
        entity.ToTable("stack_projects");
        entity.HasKey(e => new { e.IdStack, e.IdProject });
        entity.HasOne(e => e.Stack)
              .WithMany(s => s.StackProjects)
              .HasForeignKey(e => e.IdStack);
        entity.HasOne(e => e.Project)
              .WithMany(p => p.StackProjects)
              .HasForeignKey(e => e.IdProject);
    }
}