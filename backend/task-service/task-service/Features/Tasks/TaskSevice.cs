using task_service.Core.DTOs;

namespace task_service.Features.Tasks;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repo;
    public TaskService(ITaskRepository repo) => _repo = repo;

    public async Task<TaskDto> CreateAsync(Guid userId, CreateTaskRequest request)
    {
        var difficulty = Enum.Parse<TaskDifficulty>(request.Difficulty, true);
        
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate.ToUniversalTime(),
            CharacteristicId = request.CharacteristicId,
            Difficulty = difficulty,
            IsImportant = request.IsImportant,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repo.CreateAsync(task);
        return MapToDto(task);
    }

    public async Task<TaskDto> GetByIdAsync(Guid id, Guid userId)
    {
        var task = await _repo.GetByIdAsync(id, userId);
        if (task == null) throw new KeyNotFoundException("Task not found");
        return MapToDto(task);
    }

    public async Task<List<TaskDto>> GetByDateAsync(Guid userId, DateTime date)
    {
        var tasks = await _repo.GetByUserAndDateAsync(userId, date);
        return tasks.Select(MapToDto).ToList();
    }

    public async Task<List<TaskDto>> GetByDateRangeAsync(Guid userId, DateTime from, DateTime to)
    {
        var tasks = await _repo.GetByUserAndDateRangeAsync(userId, from, to);
        return tasks.Select(MapToDto).ToList();
    }

    public async Task<TaskDto> UpdateAsync(Guid id, Guid userId, UpdateTaskRequest request)
    {
        var task = await _repo.GetByIdAsync(id, userId);
        if (task == null) throw new KeyNotFoundException("Task not found");

        if (request.Title != null) task.Title = request.Title;
        if (request.Description != null) task.Description = request.Description;
        if (request.DueDate.HasValue) task.DueDate = request.DueDate.Value.ToUniversalTime();
        if (request.CharacteristicId != null) task.CharacteristicId = request.CharacteristicId;
        if (request.Difficulty != null) task.Difficulty = Enum.Parse<TaskDifficulty>(request.Difficulty, true);
        if (request.IsImportant.HasValue) task.IsImportant = request.IsImportant.Value;
        
        task.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(task);
        return MapToDto(task);
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var task = await _repo.GetByIdAsync(id, userId);
        if (task == null) throw new KeyNotFoundException("Task not found");
        await _repo.DeleteAsync(task);
    }

    public async Task<TaskDto> CompleteAsync(Guid id, Guid userId)
    {
        var task = await _repo.GetByIdAsync(id, userId);
        if (task == null) throw new KeyNotFoundException("Task not found");
        
        if (task.IsCompleted)
            throw new InvalidOperationException("Task already completed");

        task.IsCompleted = true;
        task.CompletedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(task);
        
        // TODO: Публиковать событие TaskCompleted в RabbitMQ
        
        return MapToDto(task);
    }

    public async Task<DashboardDto> GetDashboardAsync(Guid userId)
    {
        var todayTasks = await _repo.GetTodayTasksAsync(userId);
        var completed = todayTasks.Count(t => t.IsCompleted);
        var allCompleted = todayTasks.Count > 0 && completed == todayTasks.Count;

        return new DashboardDto(
            todayTasks.Select(MapToDto).ToList(),
            completed,
            todayTasks.Count,
            allCompleted
        );
    }

    private static TaskDto MapToDto(TaskItem task) => new(
        task.Id,
        task.UserId,
        task.Title,
        task.Description,
        task.DueDate,
        task.CharacteristicId,
        task.Difficulty.ToString(),
        task.IsImportant,
        task.IsCompleted,
        task.CompletedAt,
        task.CreatedAt
    );
}