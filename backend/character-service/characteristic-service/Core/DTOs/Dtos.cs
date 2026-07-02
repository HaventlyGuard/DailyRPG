namespace character_service.Core.DTOs;

public record CreateCharacteristicRequest(string Name, string Category = "General");
public record CharacteristicDto(
    Guid Id,
    string Name,
    string Category,
    int Experience,
    int Level,
    int ExperienceToNextLevel
);

public record UpdateCharacteristicRequest(string? Name, string? Category);
public record CreateRankRequest(string Name, int MinTotalLevels, int MaxTotalLevels);
public record UpdateRankRequest(string? Name, int? MinTotalLevels, int? MaxTotalLevels);
public record RankDto(Guid Id, string Name, int MinTotalLevels, int MaxTotalLevels);
public record UserRankDto(string RankName, int TotalLevels);