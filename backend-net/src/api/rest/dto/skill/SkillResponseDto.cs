namespace AndrezOG.Api.Rest.Dto.Skill;

using AndrezOG.Domain.Model.Skills;

public class SkillResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public SkillType SkillType { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public string? ImageUrl { get; set; }
}

public class SkillCardDto
{
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}

public class SkillOptionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class SkillImageResponseDto
{
    public string? ImageUrl { get; set; }
}
