using character_service.Features.Ranks;

namespace character_service.Features.Ranks;

public interface IRankRepository
{
    Task<List<Rank>> GetAllAsync();
    Task<Rank?> GetByIdAsync(Guid id);
    Task<Rank> CreateAsync(Rank rank);
    Task<Rank> UpdateAsync(Rank rank);
    Task DeleteAsync(Rank rank);
    Task<Rank?> GetByTotalLevelsAsync(int totalLevels);
}