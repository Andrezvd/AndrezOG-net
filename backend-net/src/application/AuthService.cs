namespace AndrezOG.Application;

using AndrezOG.Application.Iservices;
using AndrezOG.Application.Result;
using AndrezOG.Domain.Irepository;
using AndrezOG.Domain.Model.Tenant;
using AndrezOG.Api.Rest.Dto.Auth;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _repository;
    private readonly IConfiguration _configuration;

    public AuthService(IAuthRepository repository, IConfiguration configuration)
    {
        _repository = repository;
        _configuration = configuration;
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        // Devolvemos error si el correo ya existe
        if (await _repository.EmailExistsAsync(request.Email))
        {
            return new AuthResult
            {
                Success = false,
                Message = "El email ya esta registrado"
            };
        }

        // Creamos el usuario si el correo no existe
        var user = new User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = UserRole.Client,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Persistimos el usuario en la base de datos
        await _repository.CreateAsync(user);

        // Generamos el token JWT para el usuario registrado
        var token = GenerateJwtToken(user);
        return new AuthResult
        {
            Success = true,
            Message = "Usuario registrado exitosamente",
            Token = token,
            Email = user.Email,
            Role = user.Role.ToString()
        };
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        // Buscamos el usuario por correo electrónico
        var user = await _repository.GetByEmailAsync(request.Email);

        if (user == null)
        {
            return new AuthResult
            {
                Success = false,
                Message = "Email o contraseña incorrectos"
            };
        }

        // Verificamos la contraseña
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return new AuthResult
            {
                Success = false,
                Message = "Email o contraseña incorrectos"
            };
        }

        // Generamos el token JWT para el usuario autenticado
        var token = GenerateJwtToken(user);
        return new AuthResult
        {
            Success = true,
            Message = "Usuario autenticado exitosamente",
            Token = token,
            Email = user.Email,
            Role = user.Role.ToString()
        };
    }

    private string GenerateJwtToken(User user)
    {
        var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY")
            ?? _configuration["Jwt:Key"]
            ?? "ClaveDeDesarrolloLocal-SoloParaFallback-2025!";

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "AndrezOG",
            audience: _configuration["Jwt:Audience"] ?? "AndrezOG-App",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}