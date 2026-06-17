namespace AndrezOG.Application;

using AndrezOG.Application.Commands;
using AndrezOG.Application.Iservices;
using AndrezOG.Domain.Irepository;
using AndrezOG.Domain.Model.Tenant;

public class ProfileService : IProfileService
{
    private readonly IProfileRepository _repository;

    public ProfileService(IProfileRepository repository)
    {
        _repository = repository;
    }

    public async Task CreateDefaultProfileAsync(CreateDefaultProfileCommand command)
    {
        var profile = new Profile
        {
            IdUser = command.UserId,
            Name = $"{command.Name} {command.LastName}".Trim(),
            Title = string.Empty,
            Education = string.Empty,
            EducationStartYear = string.Empty,
            EducationEndYear = string.Empty,
            Email = command.Email,
            Available = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repository.CreateAsync(profile);
    }

    public async Task UpdateProfileAsync(UpdateProfileCommand command)
    {
        var profile = await _repository.GetByUserIdAsync(command.UserId);

        if (profile is null)
        {
            throw new KeyNotFoundException($"No profile found for user {command.UserId}.");
        }

        if (command.Name is not null) profile.Name = command.Name;
        if (command.LastName is not null) profile.LastName = command.LastName;
        if (command.PhoneNumber is not null) profile.PhoneNumber = command.PhoneNumber;
        if (command.Country is not null) profile.Country = command.Country;
        if (command.City is not null) profile.City = command.City;
        if (command.State is not null) profile.State = command.State;
        if (command.ZipCode is not null) profile.ZipCode = command.ZipCode;
        if (command.Title is not null) profile.Title = command.Title;
        if (command.Summary is not null) profile.Summary = command.Summary;
        if (command.Available.HasValue) profile.Available = command.Available.Value;
        if (command.AvailableText is not null) profile.AvailableText = command.AvailableText;
        if (command.Education is not null) profile.Education = command.Education;
        if (command.EducationStartYear is not null) profile.EducationStartYear = command.EducationStartYear;
        if (command.EducationEndYear is not null) profile.EducationEndYear = command.EducationEndYear;
        if (command.PhotoUrl is not null) profile.PhotoUrl = command.PhotoUrl;
        if (command.VideoUrl is not null) profile.VideoUrl = command.VideoUrl;
        if (command.Email is not null) profile.Email = command.Email;
        if (command.LinkedInUrl is not null) profile.LinkedInUrl = command.LinkedInUrl;
        if (command.GitHubUrl is not null) profile.GitHubUrl = command.GitHubUrl;

        profile.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(profile);
    }

    public async Task<Profile?> GetMyProfileAsync()
    {
        return await _repository.GetMyProfileAsync();
    }

    public async Task<Profile?> GetPublicProfileAsync()
    {
        return await _repository.GetPublicProfileAsync();
    }
}
