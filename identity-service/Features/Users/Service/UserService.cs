namespace identity_service.Features.User;

public class UserService : IUserService
{
    private IHttpContextAccessor _accessor;
    
    
    public async Task<User> GetOrCreateUser()
    {
        throw new NotImplementedException();
    }

    public async Task<User> GetUserBySub(string sub)
    {
        throw new NotImplementedException();
    }
}