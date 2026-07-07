namespace identity_service.Features.User;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public byte[]? Avatar { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "player";
}