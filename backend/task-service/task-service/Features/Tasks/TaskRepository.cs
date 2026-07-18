using Microsoft.EntityFrameworkCore;
using task_service.Core.DataAccess;
using task_service.Features.Tasks;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;
    public TaskRepository(AppDbContext context) => _context = context;

    public async Task<TaskItem> CreateAsync(TaskItem task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id, Guid userId)
        => await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

    public async Task<List<TaskItem>> GetByUserAndDateAsync(Guid userId, DateTime date)
        => await _context.Tasks
            .Where(t => t.UserId == userId && t.DueDate.Date == date.Date)
            .OrderBy(t => t.IsImportant ? 0 : 1)
            .ThenBy(t => t.Difficulty)
            .ToListAsync();

    public async Task<List<TaskItem>> GetByUserAndDateRangeAsync(Guid userId, DateTime from, DateTime to)
        => await _context.Tasks
            .Where(t => t.UserId == userId && t.DueDate.Date >= from.Date && t.DueDate.Date <= to.Date)
            .ToListAsync();

    public async Task<TaskItem> UpdateAsync(TaskItem task)
    {
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task DeleteAsync(TaskItem task)
    {
        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
    }

    public async Task<List<TaskItem>> GetTodayTasksAsync(Guid userId)
        => await GetByUserAndDateAsync(userId, DateTime.UtcNow);
}