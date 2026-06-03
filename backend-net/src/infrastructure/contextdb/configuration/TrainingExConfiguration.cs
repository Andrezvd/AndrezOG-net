namespace AndrezOG.Infrastructure.ContextDb.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AndrezOG.Domain.Model.Room;

public class TrainingExConfiguration : IEntityTypeConfiguration<TrainingExercise>
{
    public void Configure(EntityTypeBuilder<TrainingExercise> entity)
    {
        entity.ToTable("training_exercises");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();
        entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
        entity.Property(e => e.Description).HasMaxLength(500);
        entity.Property(e => e.Difficulty).IsRequired();
        entity.Property(e => e.InitialCodeCsharp).HasMaxLength(2000);
        entity.Property(e => e.InitialCodePython).HasMaxLength(2000);
        entity.Property(e => e.InitialCodeJava).HasMaxLength(2000);
        entity.Property(e => e.InitialCodeTypeScript).HasMaxLength(2000);
        entity.Property(e => e.TestCases).HasMaxLength(2000);
        entity.Property(e => e.Solution).HasMaxLength(2000);
        entity.Property(e => e.InputExample).HasMaxLength(1000);
        entity.Property(e => e.OutputExample).HasMaxLength(1000);
        entity.Property(e => e.ImagesExample).HasMaxLength(2000);
        entity.Property(e => e.IsActive).IsRequired();
        entity.Property(e => e.Order).IsRequired();
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
    }
}