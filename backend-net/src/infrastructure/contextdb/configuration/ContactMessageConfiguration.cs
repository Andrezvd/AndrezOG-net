namespace AndrezOG.Infrastructure.ContextDb.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AndrezOG.Domain.Model;

public class ContactMessageConfiguration : IEntityTypeConfiguration<ContactMessage>
{
    public void Configure(EntityTypeBuilder<ContactMessage> entity)
    {
        entity.ToTable("contact_messages");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();
        entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
        entity.Property(e => e.Message).IsRequired().HasMaxLength(1000);
        entity.Property(e => e.Type).IsRequired();
        entity.Property(e => e.IsRead).IsRequired();
        entity.Property(e => e.NeedsAttention).IsRequired();
        entity.Property(e => e.CreatedAt).IsRequired();
    }
}