using task_service.Core.DTOs;
using task_service.Features.Tasks;

namespace task_service.Features.Tasks;

public interface ITaskService
{
    Task<TaskDto> CreateAsync(Guid userId, CreateTaskRequest request);
    Task<TaskDto> GetByIdAsync(Guid id, Guid userId);
    Task<List<TaskDto>> GetByDateAsync(Guid userId, DateTime date);
    Task<List<TaskDto>> GetByDateRangeAsync(Guid userId, DateTime from, DateTime to);
    Task<TaskDto> UpdateAsync(Guid id, Guid userId, UpdateTaskRequest request);
    Task DeleteAsync(Guid id, Guid userId);
    Task<TaskDto> CompleteAsync(Guid id, Guid userId);
    Task<DashboardDto> GetDashboardAsync(Guid userId);
}