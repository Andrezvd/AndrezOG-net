namespace AndrezOG.Domain.Irepository;

using AndrezOG.Domain.Model.Tenant;

public interface IProfileRepository
{
    Task<Profile?> GetByUserIdAsync(int userId);
    Task CreateAsync(Profile profile);
    Task UpdateAsync(Profile profile);
    Task<Profile?> GetPublicProfileAsync();
}
