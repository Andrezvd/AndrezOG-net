namespace AndrezOG.Api.Rest.Dto.Profile;

public class MyProfileDto
{
    public string Name { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Country { get; set; } = null!;
    public string City { get; set; } = null!;
    public string State { get; set; } = null!;
    public string ZipCode { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Summary { get; set; } = null!;
    public bool Available { get; set; }
    public string AvailableText { get; set; } = null!;
    public string Education { get; set; } = null!;
    public string EducationStartYear { get; set; } = null!;
    public string EducationEndYear { get; set; } = null!;
    public string PhotoUrl { get; set; } = null!;
    public string VideoUrl { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string LinkedInUrl { get; set; } = null!;
    public string GitHubUrl { get; set; } = null!;
}