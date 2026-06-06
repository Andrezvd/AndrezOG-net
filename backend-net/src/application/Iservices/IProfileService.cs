namespace AndrezOG.Application.Iservices;

using AndrezOG.Application.Commands;

public interface IProfileService
{
    Task CreateDefaultProfileAsync(CreateDefaultProfileCommand command);
}
