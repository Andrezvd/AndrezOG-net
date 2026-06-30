namespace AndrezOG.Application.Iservices;

using AndrezOG.Application.Commands;
using AndrezOG.Domain.Model.Tenant;
public interface IProfileService
{
    Task CreateDefaultProfileAsync(CreateDefaultProfileCommand command);
    Task UpdateProfileAsync(UpdateProfileCommand command);
    Task<Profile?> GetByUserIdAsync(int userId);
    Task<Profile?> GetPublicProfileAsync();
}
