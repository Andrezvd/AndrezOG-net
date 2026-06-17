namespace AndrezOG.Api.Rest.Mapper.Profile;

using AndrezOG.Application.Commands;
using AndrezOG.Api.Rest.Dto.Profile;
using AndrezOG.Domain.Model.Tenant;
public static class ProfileMapper
{
    public static UpdateProfileCommand ToUpdateProfileCommand(int userId, UpdateRequest requestdto)
    {
        return new UpdateProfileCommand(
            userId,
            requestdto.Name,
            requestdto.LastName,
            requestdto.PhoneNumber,
            requestdto.Country,
            requestdto.City,
            requestdto.State,
            requestdto.ZipCode,
            requestdto.Title,
            requestdto.Summary,
            requestdto.Available,
            requestdto.AvailableText,
            requestdto.Education,
            requestdto.EducationStartYear,
            requestdto.EducationEndYear,
            requestdto.PhotoUrl,
            requestdto.VideoUrl,
            requestdto.Email,
            requestdto.LinkedInUrl,
            requestdto.GitHubUrl
        );
    }

    public static MyProfileDto DomainToDto(Profile profile)
    {
        return new MyProfileDto
        {
            Name = profile.Name,
            LastName = profile.LastName,
            PhoneNumber = profile.PhoneNumber,
            Country = profile.Country,
            City = profile.City,
            State = profile.State,
            ZipCode = profile.ZipCode,
            Title = profile.Title,
            Summary = profile.Summary ?? string.Empty,
            Available = profile.Available,
            AvailableText = profile.AvailableText ?? string.Empty,
            Education = profile.Education ?? string.Empty,
            EducationStartYear = profile.EducationStartYear ?? string.Empty,
            EducationEndYear = profile.EducationEndYear ?? string.Empty,
            PhotoUrl = profile.PhotoUrl ?? string.Empty,
            VideoUrl = profile.VideoUrl ?? string.Empty,
            Email = profile.Email ?? string.Empty,
            LinkedInUrl = profile.LinkedInUrl ?? string.Empty,
            GitHubUrl = profile.GitHubUrl ?? string.Empty
        };
    }
}