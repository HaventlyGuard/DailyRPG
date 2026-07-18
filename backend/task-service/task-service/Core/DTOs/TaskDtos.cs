namespace task_service.Core.DTOs;

public record CreateTaskRequest(
    string Title,
    string? Description,
    DateTime DueDate,
    Guid? CharacteristicId = null,
    string Difficulty = "Medium",
    bool IsImportant = false
);

public record UpdateTaskRequest(
    string? Title,
    string? Description,
    DateTime? DueDate,
    Guid? CharacteristicId,
    string? Difficulty,
    bool? IsImportant
);

public record TaskDto(
    Guid Id,
    Guid UserId,
    string Title,
    string? Description,
    DateTime DueDate,
    Guid? CharacteristicId,
    string Difficulty,
    bool IsImportant,
    bool IsCompleted,
    DateTime? CompletedAt,
    DateTime CreatedAt
);

public record DashboardDto(
    List<TaskDto> TodayTasks,
    int CompletedCount,
    int TotalCount,
    bool AllCompleted
);