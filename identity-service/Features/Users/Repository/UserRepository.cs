namespace identity_service.Features.User.Repository;

public class UserRepository : IUserRepository
{
    public async Task<User> CreateUser(User user)
    {
        throw new NotImplementedException();
    }

    public async Task<User> GetUserByEmail(string email)
    {
        throw new NotImplementedException();
    }

    public async Task<User> GetUserBySub(string sub)
    {
        throw new NotImplementedException();
    }
}