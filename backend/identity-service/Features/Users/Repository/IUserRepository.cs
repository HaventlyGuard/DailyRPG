using identity_service.Features.User;

namespace identity_service.Features.User.Repository;

public interface IUserRepository
{
    Task<User> CreateUser(User user);
    Task<User?> GetUserByEmail(string email);
    Task<User?> GetUserById(Guid userId);
    Task<User?> GetUserByUsername(string username);
    Task<User> UpdateUser(User user);
    Task DeleteUser(User user);
    Task<List<User>> GetAllUsers(int page, int pageSize);
}