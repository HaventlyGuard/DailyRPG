using System.Security.Claims;
using identity_service.Controllers;
using identity_service.Core.Constants;
using identity_service.Core.DTOs;
using identity_service.Features.User.Repository;

namespace identity_service.Features.User;

public class UserService : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;

    public UserService(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
    }

    public async Task<User> GetOrCreateUser()
    {
        var httpUser = _httpContextAccessor.HttpContext?.User;
        if (httpUser == null)
            throw new UnauthorizedAccessException("User not authenticated");

        var userId = httpUser.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = httpUser.FindFirstValue(ClaimTypes.Email);
        var username = httpUser.FindFirstValue("preferred_username");

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email))
            throw new InvalidOperationException("Required claims not found");

        var existingUser = await _userRepository.GetUserById(Guid.Parse(userId));
        if (existingUser != null)
        {
            existingUser.LastLoginAt = DateTime.UtcNow;
            await _userRepository.UpdateUser(existingUser);
            return existingUser;
        }

        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Username = username ?? email.Split('@')[0],
            CreatedAt = DateTime.UtcNow,
            LastLoginAt = DateTime.UtcNow,
            IsActive = true,
            Role = "player"
        };

        return await _userRepository.CreateUser(newUser);
    }

    public async Task<User?> GetUserById(Guid userId)
    {
        return await _userRepository.GetUserById(userId);
    }

    public async Task<User> UpdateProfile(Guid userId, UpdateProfileRequest request)
    {
        var user = await _userRepository.GetUserById(userId);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        if (!string.IsNullOrEmpty(request.Username))
            user.Username = request.Username;
        
        if (!string.IsNullOrEmpty(request.AvatarUrl))
            user.AvatarUrl = request.AvatarUrl;

        user.LastLoginAt = DateTime.UtcNow;

        return await _userRepository.UpdateUser(user);
    }

    public async Task<List<User>> GetAllUsers(int page, int pageSize)
    {
        return await _userRepository.GetAllUsers(page, pageSize);
    }

    public async Task DeleteUser(Guid userId)
    {
        var user = await _userRepository.GetUserById(userId);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        await _userRepository.DeleteUser(user);
    }
    
    
    public async Task<User> ChangeUserRole(Guid userId, string newRole)
    {
        if (!new[] { UserRoles.Player, UserRoles.Admin, UserRoles.Moderator }.Contains(newRole))
            throw new ArgumentException("Invalid role");

        var user = await _userRepository.GetUserById(userId);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        user.Role = newRole;
        return await _userRepository.UpdateUser(user);
    }

    public async Task<object> GetUserStats()
    {
        var users = await _userRepository.GetAllUsers(1, int.MaxValue);
    
        return new
        {
            TotalUsers = users.Count,
            ActiveUsers = users.Count(u => u.IsActive),
            Admins = users.Count(u => u.Role == UserRoles.Admin),
            NewToday = users.Count(u => u.CreatedAt.Date == DateTime.UtcNow.Date),
            Roles = users.GroupBy(u => u.Role)
                .Select(g => new { Role = g.Key, Count = g.Count() })
        };
    }
}

