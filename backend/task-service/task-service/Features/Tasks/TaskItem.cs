namespace task_service.Features.Tasks;

public class TaskItem
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public Guid? CharacteristicId { get; set; }  
    public TaskDifficulty Difficulty { get; set; } = TaskDifficulty.Medium;
    public bool IsImportant { get; set; }          
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public enum TaskDifficulty
{
    Easy = 1,
    Medium = 2,
    Hard = 3,
    Epic = 4
}