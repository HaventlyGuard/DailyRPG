using Microsoft.EntityFrameworkCore;
using task_service.Core.DataAccess;

namespace task_service.Features.Tasks;

public interface ITaskRepository
{
    Task<TaskItem> CreateAsync(TaskItem task);
    Task<TaskItem?> GetByIdAsync(Guid id, Guid userId);
    Task<List<TaskItem>> GetByUserAndDateAsync(Guid userId, DateTime date);
    Task<List<TaskItem>> GetByUserAndDateRangeAsync(Guid userId, DateTime from, DateTime to);
    Task<TaskItem> UpdateAsync(TaskItem task);
    Task DeleteAsync(TaskItem task);
    Task<List<TaskItem>> GetTodayTasksAsync(Guid userId);
}

