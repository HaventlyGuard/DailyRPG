using character_service.Core.DTOs;

namespace character_service.Features.Ranks;

public interface IRankService
{
    Task<List<RankDto>> GetAllAsync();
    Task<RankDto> CreateAsync(CreateRankRequest request);
    Task<RankDto> UpdateAsync(Guid id, UpdateRankRequest request);
    Task DeleteAsync(Guid id);
    Task<Rank?> GetRankByTotalLevelsAsync(int totalLevels);
}