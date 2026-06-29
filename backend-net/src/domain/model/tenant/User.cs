namespace AndrezOG.Domain.Model.Tenant;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public UserRole Role { get; set; } = UserRole.Client;
    public AuthProvider Provider { get; set; } = AuthProvider.Local;
    public string? ExternalId { get; set; }
    public string? PhoneNumber { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public string? TwoFactorSecret { get; set; }

    // Refresh token
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpires { get; set; }

    // Account lockout
    public int FailedAttempts { get; set; }
    public DateTime? LockoutEnd { get; set; }

    // Email verification
    public bool EmailVerified { get; set; }
    public string? EmailVerificationToken { get; set; }
}
