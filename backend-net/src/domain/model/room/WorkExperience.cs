namespace AndrezOG.Domain.Model.Room;

public class WorkExperience
{
    public int Id { get; set; }
    public string Company { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime EndDate { get; set; } = DateTime.UtcNow.AddMonths(6);
    public string? CompanyUrl { get; set; }
    public string? CompanyLogoUrl { get; set; }
    public bool IsCurrent { get; set; } = false;
    public string? Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int Order { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}