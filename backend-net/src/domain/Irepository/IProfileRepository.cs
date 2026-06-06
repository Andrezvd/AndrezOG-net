namespace AndrezOG.Domain.Irepository;

using AndrezOG.Domain.Model.Tenant;

public interface IProfileRepository
{
    Task CreateAsync(Profile profile);
}
