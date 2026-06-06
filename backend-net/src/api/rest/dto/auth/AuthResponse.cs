namespace AndrezOG.Api.Rest.Dto.Auth;

public record AuthResponse(
    string Message,
    string Token,
    string Email,
    string Role
);
