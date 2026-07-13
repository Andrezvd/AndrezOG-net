namespace AndrezOG.Application.Dto.Stack;

public class StackAppDto
{
    public int Id { get; set; }
    public string Summary { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<SkillRefAppDto> Skills { get; set; } = new();
}

public class SkillRefAppDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}