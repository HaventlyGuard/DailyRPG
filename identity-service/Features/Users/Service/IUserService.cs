namespace identity_service.Features.User;

public interface IUserService
{
    public Task<User> GetOrCreateUser();
    public Task<User> GetUserBySub(string sub);
}