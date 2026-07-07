using identity_service.Core.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace identity_service.Features.User.Repository;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User> CreateUser(User user)
    {
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetUserById(Guid userId)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<User?> GetUserByUsername(string username)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User> UpdateUser(User user)
    {
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public async Task DeleteUser(User user)
    {
        user.IsActive = false;
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<User>> GetAllUsers(int page, int pageSize)
    {
        return await _dbContext.Users
            .Where(u => u.IsActive)
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}