namespace AndrezOG.Application;

using AndrezOG.Application.Commands;
using AndrezOG.Application.Iservices;
using AndrezOG.Application.Result;
using AndrezOG.Domain.Irepository;
using AndrezOG.Domain.Model.Tenant;
using AndrezOG.Infrastructure.Auth;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _repository;
    private readonly IConfiguration _configuration;
    private readonly GoogleAuthService _googleAuth;

    public AuthService(
        IAuthRepository repository,
        IConfiguration configuration,
        GoogleAuthService googleAuth)
    {
        _repository = repository;
        _configuration = configuration;
        _googleAuth = googleAuth;
    }

    // ================================================================
    // REGISTRO MANUAL (email + password)
    // ================================================================

    public async Task<AuthResult> RegisterAsync(RegisterCommand command)
    {
        if (!IsPasswordStrong(command.Password))
        {
            return new AuthResult
            {
                Success = false,
                Message = "La contraseña debe tener al menos 8 caracteres, 1 mayúscula, 1 número y 1 carácter especial."
            };
        }

        if (await _repository.EmailExistsAsync(command.Email))
        {
            return new AuthResult
            {
                Success = false,
                Message = "El email ya está registrado"
            };
        }

        var adminEmails = _configuration.GetSection("AdminEmails").Get<string[]>() ?? Array.Empty<string>();
        var role = adminEmails.Contains(command.Email, StringComparer.OrdinalIgnoreCase)
            ? UserRole.Admin
            : UserRole.Client;

        var user = new User
        {
            Email = command.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password),
            Role = role,
            EmailVerified = false,
            EmailVerificationToken = GenerateVerificationToken(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repository.CreateAsync(user);

        var token = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpires = DateTime.UtcNow.AddDays(7);
        await _repository.UpdateAsync(user);

        return new AuthResult
        {
            Success = true,
            Message = "Usuario registrado exitosamente",
            UserId = user.Id,
            Email = user.Email,
            Name = string.Empty,
            Role = user.Role.ToString(),
            Token = token,
            RefreshToken = refreshToken
        };
    }

    // ================================================================
    // LOGIN MANUAL (email + password)
    // ================================================================

    public async Task<AuthResult> LoginAsync(LoginCommand command)
    {
        var user = await _repository.GetByEmailAsync(command.Email);

        if (user == null)
        {
            return new AuthResult
            {
                Success = false,
                Message = "Email o contraseña incorrectos"
            };
        }

        // Verificar lockout
        if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
        {
            var remaining = user.LockoutEnd.Value - DateTime.UtcNow;
            return new AuthResult
            {
                Success = false,
                Message = $"Cuenta bloqueada temporalmente. Intenta de nuevo en {remaining.Minutes} minuto(s)."
            };
        }

        if (!BCrypt.Net.BCrypt.Verify(command.Password, user.PasswordHash))
        {
            user.FailedAttempts++;

            if (user.FailedAttempts >= 5)
            {
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
                user.FailedAttempts = 0;
                await _repository.UpdateAsync(user);
                return new AuthResult
                {
                    Success = false,
                    Message = "Cuenta bloqueada por 15 minutos debido a múltiples intentos fallidos."
                };
            }

            await _repository.UpdateAsync(user);
            return new AuthResult
            {
                Success = false,
                Message = "Email o contraseña incorrectos"
            };
        }

        // Login exitoso: resetear contadores
        user.FailedAttempts = 0;
        user.LockoutEnd = null;
        user.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(user);

        var token = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();
        await SaveRefreshToken(user, refreshToken);

        return new AuthResult
        {
            Success = true,
            Message = "Usuario autenticado exitosamente",
            UserId = user.Id,
            Email = user.Email,
            Name = string.Empty,
            Role = user.Role.ToString(),
            Token = token,
            RefreshToken = refreshToken
        };
    }

    // ================================================================
    // GOOGLE OAUTH LOGIN / REGISTRO
    // ================================================================

    public async Task<AuthResult> GoogleLoginAsync(GoogleLoginCommand command)
    {
        // 1. Intercambiar code por token con Google
        GoogleTokenResponse tokenResponse;
        try
        {
            tokenResponse = await _googleAuth.ExchangeCodeAsync(command.Code, command.RedirectUri);
        }
        catch (Exception ex)
        {
            return new AuthResult
            {
                Success = false,
                Message = $"Error al autenticar con Google: {ex.Message}"
            };
        }

        // 2. Obtener datos del usuario desde Google
        GoogleUserInfo userInfo;
        try
        {
            userInfo = await _googleAuth.GetUserInfoAsync(tokenResponse.AccessToken);
        }
        catch (Exception ex)
        {
            return new AuthResult
            {
                Success = false,
                Message = $"Error al obtener datos de Google: {ex.Message}"
            };
        }

        if (!userInfo.EmailVerified)
        {
            return new AuthResult
            {
                Success = false,
                Message = "El email de Google no está verificado."
            };
        }

        // 3. Buscar usuario por GoogleId o Email
        var user = await _repository.GetByGoogleIdAsync(userInfo.Sub)
                   ?? await _repository.GetByEmailAsync(userInfo.Email);

        if (user != null)
        {
            // Vincular GoogleId si no lo tiene
            if (string.IsNullOrEmpty(user.GoogleId))
            {
                user.GoogleId = userInfo.Sub;
                user.UpdatedAt = DateTime.UtcNow;
                await _repository.UpdateAsync(user);
            }
        }
        else
        {
            // Nuevo usuario
            var adminEmails = _configuration.GetSection("AdminEmails").Get<string[]>() ?? Array.Empty<string>();
            var role = adminEmails.Contains(userInfo.Email, StringComparer.OrdinalIgnoreCase)
                ? UserRole.Admin
                : UserRole.Client;

            user = new User
            {
                Email = userInfo.Email,
                GoogleId = userInfo.Sub,
                PasswordHash = string.Empty,
                Role = role,
                EmailVerified = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _repository.CreateAsync(user);
        }

        var jwtToken = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();
        await SaveRefreshToken(user, refreshToken);

        return new AuthResult
        {
            Success = true,
            Message = "Autenticado con Google exitosamente",
            UserId = user.Id,
            Email = user.Email,
            Name = userInfo.Name,
            Role = user.Role.ToString(),
            Token = jwtToken,
            RefreshToken = refreshToken
        };
    }

    // ================================================================
    // REFRESH TOKEN
    // ================================================================

    public async Task<AuthResult> RefreshTokenAsync(RefreshTokenCommand command)
    {
        var user = await _repository.GetByRefreshTokenAsync(command.RefreshToken);

        if (user == null)
        {
            return new AuthResult
            {
                Success = false,
                Message = "Refresh token inválido o expirado."
            };
        }

        var newJwt = GenerateJwtToken(user);
        var newRefreshToken = GenerateRefreshToken();
        await SaveRefreshToken(user, newRefreshToken);

        return new AuthResult
        {
            Success = true,
            Message = "Token renovado exitosamente",
            UserId = user.Id,
            Token = newJwt,
            RefreshToken = newRefreshToken,
            Email = user.Email,
            Role = user.Role.ToString()
        };
    }

    // ================================================================
    // LOGOUT
    // ================================================================

    public async Task LogoutAsync(int userId)
    {
        var user = await _repository.GetByIdAsync(userId);
        if (user != null)
        {
            user.RefreshToken = null;
            user.RefreshTokenExpires = null;
            await _repository.UpdateAsync(user);
        }
    }

    // ================================================================
    // MÉTODOS PRIVADOS
    // ================================================================

    private string GenerateJwtToken(User user)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? "ClaveDeDesarrolloLocal-SoloParaFallback-2025!";

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "AndrezOG",
            audience: _configuration["Jwt:Audience"] ?? "AndrezOG-Client",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:ExpireMinutes"] ?? "60")),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    private async Task SaveRefreshToken(User user, string refreshToken)
    {
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpires = DateTime.UtcNow.AddDays(7);
        await _repository.UpdateAsync(user);
    }

    private static string GenerateVerificationToken()
    {
        var bytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    private static bool IsPasswordStrong(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8) return false;
        if (!password.Any(char.IsUpper)) return false;
        if (!password.Any(char.IsDigit)) return false;
        if (!password.Any(c => !char.IsLetterOrDigit(c))) return false;
        return true;
    }
}