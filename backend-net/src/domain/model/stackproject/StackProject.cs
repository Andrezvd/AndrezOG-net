namespace AndrezOG.Domain.Model.StackProject;

public class StackProject
{
    public int IdStack { get; set; }
    public int IdProject { get; set; }
    public Stack Stack { get; set; } = null!;
    public Project Project { get; set; } = null!;
}