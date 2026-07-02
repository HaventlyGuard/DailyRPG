using character_service.Core.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace character_service.Features.Ranks;

public class RankRepository : IRankRepository
{
    private readonly AppDbContext _context;
    public RankRepository(AppDbContext context) => _context = context;

    public async Task<List<Rank>> GetAllAsync() => await _context.Ranks.ToListAsync();
    public async Task<Rank?> GetByIdAsync(Guid id) => await _context.Ranks.FindAsync(id);

    public async Task<Rank> CreateAsync(Rank rank)
    {
        _context.Ranks.Add(rank);
        await _context.SaveChangesAsync();
        return rank;
    }

    public async Task<Rank> UpdateAsync(Rank rank)
    {
        _context.Ranks.Update(rank);
        await _context.SaveChangesAsync();
        return rank;
    }

    public async Task DeleteAsync(Rank rank)
    {
        _context.Ranks.Remove(rank);
        await _context.SaveChangesAsync();
    }

    public async Task<Rank?> GetByTotalLevelsAsync(int totalLevels)
    {
        return await _context.Ranks
            .FirstOrDefaultAsync(r => totalLevels >= r.MinTotalLevels && totalLevels <= r.MaxTotalLevels);
    }
}