namespace AndrezOG.Api.Rest.Mapper.Auth;

using AndrezOG.Application.Commands;
using AndrezOG.Application.Result;
using AndrezOG.Api.Rest.Dto.Auth;

public static class AuthMappers
{
    public static bool PasswordsMatch(RegisterRequest request) =>
        request.Password == request.ConfirmPassword;

    public static RegisterCommand ToRegisterCommand(RegisterRequest request) =>
        new(request.Email, request.Password);

    public static CreateDefaultProfileCommand ToCreateDefaultProfileCommand(
        RegisterRequest request,
        int userId) =>
        new(
            userId,
            request.Email,
            request.Name,
            request.LastName,
            request.PhoneNumber,
            request.Country
        );

    public static AuthResponse ToAuthResponse(AuthResult result) =>
        new(
            result.Message!,
            result.Token!,
            result.UserId!.Value,
            result.Email!,
            result.Name!,
            result.Role!
        );

    public static ErrorResponse ToErrorResponse(AuthResult result) =>
        new(result.Message ?? "Error desconocido");
}
