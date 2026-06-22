namespace AndrezOG.Domain.Model.StackProject;

public class Stack
{
    public int Id { get; set; }
    public string Summary { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<StackProject> StackProjects { get; set; } = new List<StackProject>();
    public ICollection<StackSkill> StackSkills { get; set; } = new List<StackSkill>();

}