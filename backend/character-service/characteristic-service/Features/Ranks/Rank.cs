namespace character_service.Features.Ranks;

public class Rank
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MinTotalLevels { get; set; }   
    public int MaxTotalLevels { get; set; }   
}