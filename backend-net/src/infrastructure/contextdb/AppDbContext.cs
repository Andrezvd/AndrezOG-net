namespace AndrezOG.Infrastructure.ContextDb;

using AndrezOG.Domain.Model;
using Microsoft.EntityFrameworkCore;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Profile> Profiles { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<WorkExperience> Experiences { get; set; } = null!;
    public DbSet<TrainingExercise> Trainings { get; set; } = null!;
    public DbSet<Skill> Skills { get; set; } = null!;
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<Stack> Stacks { get; set; } = null!;
    public DbSet<StackProject> StackProjects { get; set; } = null!;
    public DbSet<ContactMessage> ContactMessages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}