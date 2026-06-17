namespace AndrezOG.Domain.Model.Tenant;

public class Profile
{
    public int IdUser { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public bool Available { get; set; }
    public string? AvailableText { get; set; }
    public string? Education { get; set; } = string.Empty;
    public string? EducationStartYear { get; set; } = string.Empty;
    public string? EducationEndYear { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public string? VideoUrl { get; set; }
    public string? Email { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? GitHubUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}