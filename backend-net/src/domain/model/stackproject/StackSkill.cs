namespace AndrezOG.Domain.Model.StackProject;

public class StackSkill
{
    public int IdStack { get; set; }
    public int IdSkill { get; set; }
    public Stack Stack { get; set; } = null!;
    public Skills.Skill Skill { get; set; } = null!;
    public StackRole StackRole { get; set; }
}