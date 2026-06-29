namespace AndrezOG.Api.Rest.Dto.Auth;

public record AuthResponse(
    string Message,
    string Token,
    string RefreshToken,
    int UserId,
    string Email,
    string Name,
    string Role
);
