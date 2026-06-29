namespace AndrezOG.Application.Commands;

public record UpdateProfileCommand(
    int UserId,
    string? Name,
    string? LastName,
    string? PhoneNumber,
    string? Country,
    string? City,
    string? State,
    string? ZipCode,
    string? Title,
    string? Summary,
    bool? Available,
    string? AvailableText,
    string? Education,
    string? EducationStartYear,
    string? EducationEndYear,
    string? PhotoUrl,
    string? VideoUrl,
    string? Email,
    string? LinkedInUrl,
    string? GitHubUrl
);