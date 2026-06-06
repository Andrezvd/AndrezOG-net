namespace AndrezOG.Application.Commands;

public record CreateDefaultProfileCommand(
    int UserId,
    string Email,
    string Name,
    string LastName,
    string PhoneNumber,
    string Country
);
