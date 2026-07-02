using character_service.Core.DTOs;
using character_service.Features.Ranks;

namespace character_service.Features.Characteristics;

public class CharacteristicService : ICharacteristicService
{
    private readonly ICharacteristicRepository _repo;
    private readonly IRankService _rankService;

    public CharacteristicService(ICharacteristicRepository repo, IRankService rankService)
    {
        _repo = repo;
        _rankService = rankService;
    }

    public async Task<CharacteristicDto> CreateAsync(Guid userId, CreateCharacteristicRequest request)
    {
        var characteristic = new Characteristic
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = request.Name,
            Category = request.Category,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _repo.CreateAsync(characteristic);
        return MapToDto(characteristic);
    }

    public async Task<List<CharacteristicDto>> GetByUserAsync(Guid userId)
    {
        var list = await _repo.GetByUserAsync(userId);
        return list.Select(MapToDto).ToList();
    }

    public async Task<CharacteristicDto> GetByIdAsync(Guid id, Guid userId)
    {
        var c = await _repo.GetByIdAsync(id, userId);
        if (c == null) throw new KeyNotFoundException("Characteristic not found");
        return MapToDto(c);
    }

    public async Task<CharacteristicDto> UpdateAsync(Guid id, Guid userId, UpdateCharacteristicRequest request)
    {
        var c = await _repo.GetByIdAsync(id, userId);
        if (c == null) throw new KeyNotFoundException("Characteristic not found");
        if (request.Name != null) c.Name = request.Name;
        if (request.Category != null) c.Category = request.Category;
        c.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(c);
        return MapToDto(c);
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var c = await _repo.GetByIdAsync(id, userId);
        if (c == null) throw new KeyNotFoundException("Characteristic not found");
        await _repo.DeleteAsync(c);
    }

    public async Task AddExperienceAsync(Guid characteristicId, Guid userId, int amount)
    {
        var c = await _repo.GetByIdAsync(characteristicId, userId);
        if (c == null) throw new KeyNotFoundException("Characteristic not found");
        c.Experience += amount;
        while (c.CanLevelUp)
        {
            c.Experience -= c.ExperienceToNextLevel;
            c.Level++;
        }
        c.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(c);
    }

    public async Task<UserRankDto> GetUserRankAsync(Guid userId)
    {
        var totalLevels = await _repo.GetTotalLevelsAsync(userId);
        var rank = await _rankService.GetRankByTotalLevelsAsync(totalLevels);
        return new UserRankDto(rank?.Name ?? "Новичок", totalLevels);
    }

    private static CharacteristicDto MapToDto(Characteristic c)
        => new(c.Id, c.Name, c.Category, c.Experience, c.Level, c.ExperienceToNextLevel);
}