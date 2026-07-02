namespace character_service.Features.Characteristics;

public class Characteristic
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = "General";
    public int Experience { get; set; }
    public int Level { get; set; } = 1;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public int ExperienceToNextLevel => Level * 1000;
    public bool CanLevelUp => Experience >= ExperienceToNextLevel;
}