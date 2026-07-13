namespace AndrezOG.Api.Rest.Dto.Stack;

public class StackResponseDto
{
    public int Id { get; set; }
    public string Summary { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<SkillRefResponseDto> Skills { get; set; } = new();
}

public class SkillRefResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}

/// <summary>
/// DTO público simplificado para vitrina (stack + skills).
/// </summary>
public class StackCardDto
{
    public int Id { get; set; }
    public string Summary { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public List<SkillRefResponseDto> Skills { get; set; } = new();
}