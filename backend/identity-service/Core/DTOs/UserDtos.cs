namespace identity_service.Core.DTOs;

public record RegisterRequest(string Username, string Email, string Password);
public record LoginRequest(string Email, string Password);
public record AuthResponse(string Token, UserDto User);

public record UpdateProfileRequest(string? Username, string? AvatarUrl);
public record ChangeRoleRequest(string NewRole);

public record UserDto(
    Guid Id,
    string Username,
    string Email,
    string? AvatarUrl,
    DateTime CreatedAt,
    DateTime LastLoginAt,
    bool IsActive,
    string Role
);