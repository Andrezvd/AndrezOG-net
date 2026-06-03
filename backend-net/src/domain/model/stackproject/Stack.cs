namespace AndrezOG.Domain.Model.StackProject;

public class Stack
{
    public int Id { get; set; }
    public string Summary { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Backend { get; set; } = string.Empty;
    public string Frontend { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
    public string Cloud { get; set; } = string.Empty;
    public string Conteinerization { get; set; } = string.Empty;
    public string? Cicd { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<StackProject> StackProjects { get; set; } = new List<StackProject>();

}