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
}
