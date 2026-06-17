namespace AndrezOG.Api.Rest.Dto.Profile;

public class UpdateRequest
{
    public string? Name { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? Title { get; set; }
    public string? Summary { get; set; }
    public bool? Available { get; set; }
    public string? AvailableText { get; set; }
    public string? Education { get; set; }
    public string? EducationStartYear { get; set; }
    public string? EducationEndYear { get; set; }
    public string? PhotoUrl { get; set; }
    public string? VideoUrl { get; set; }
    public string? Email { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? GitHubUrl { get; set; }
}