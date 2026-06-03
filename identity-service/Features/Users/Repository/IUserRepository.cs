namespace identity_service.Features.User.Repository;

public interface IUserRepository
{
    public Task<User> CreateUser(User user);
    public Task<User> GetUserByEmail(string email);
    public Task<User> GetUserBySub(string sub);
}