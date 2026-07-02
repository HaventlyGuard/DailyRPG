using character_service.Core.DTOs;
using character_service.Features.Ranks;

namespace character_service.Features.Ranks;

public class RankService : IRankService
{
    private readonly IRankRepository _repo;
    public RankService(IRankRepository repo) => _repo = repo;

    public async Task<List<RankDto>> GetAllAsync()
    {
        var ranks = await _repo.GetAllAsync();
        return ranks.Select(r => new RankDto(r.Id, r.Name, r.MinTotalLevels, r.MaxTotalLevels)).ToList();
    }

    public async Task<RankDto> CreateAsync(CreateRankRequest request)
    {
        var rank = new Rank { Id = Guid.NewGuid(), Name = request.Name, MinTotalLevels = request.MinTotalLevels, MaxTotalLevels = request.MaxTotalLevels };
        await _repo.CreateAsync(rank);
        return new RankDto(rank.Id, rank.Name, rank.MinTotalLevels, rank.MaxTotalLevels);
    }

    public async Task<RankDto> UpdateAsync(Guid id, UpdateRankRequest request)
    {
        var rank = await _repo.GetByIdAsync(id);
        if (rank == null) throw new KeyNotFoundException("Rank not found");
        if (request.Name != null) rank.Name = request.Name;
        if (request.MinTotalLevels.HasValue) rank.MinTotalLevels = request.MinTotalLevels.Value;
        if (request.MaxTotalLevels.HasValue) rank.MaxTotalLevels = request.MaxTotalLevels.Value;
        await _repo.UpdateAsync(rank);
        return new RankDto(rank.Id, rank.Name, rank.MinTotalLevels, rank.MaxTotalLevels);
    }

    public async Task DeleteAsync(Guid id)
    {
        var rank = await _repo.GetByIdAsync(id);
        if (rank == null) throw new KeyNotFoundException("Rank not found");
        await _repo.DeleteAsync(rank);
    }

    public async Task<Rank?> GetRankByTotalLevelsAsync(int totalLevels)
        => await _repo.GetByTotalLevelsAsync(totalLevels);
}