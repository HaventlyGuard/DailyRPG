using identity_service.Core.DTOs;
using identity_service.Features.User;

namespace identity_service.Features.User;

public interface IUserService
{
    Task<User> GetOrCreateUser();
    Task<User?> GetUserById(Guid userId);
    Task<User> UpdateProfile(Guid userId, UpdateProfileRequest request);
    Task<User> ChangeUserRole(Guid userId, string newRole);
    Task<List<User>> GetAllUsers(int page, int pageSize);
    Task DeleteUser(Guid userId);
    Task<object> GetUserStats();
}