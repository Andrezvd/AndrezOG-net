namespace AndrezOG.Api.Rest.Dto.Auth;

public record AuthResponse
(
    string Token,
    string Email,
    string Role
);