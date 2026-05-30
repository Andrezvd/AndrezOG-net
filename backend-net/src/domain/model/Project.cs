namespace AndrezOG.Domain.Model;

public class Project
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime EndDate { get; set; } = DateTime.UtcNow.AddMonths(6);
    public bool IsActive { get; set; } = true;
    public string? RepositoryUrl { get; set; } = string.Empty;
    public ProjectType Type { get; set; } = ProjectType.Personal;
    public string? ImageUrl { get; set; } = string.Empty;
    public ICollection<StackProject> StackProjects { get; set; } = new List<StackProject>();

}