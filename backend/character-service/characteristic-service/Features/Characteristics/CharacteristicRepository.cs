using character_service.Core.DataAccess;

namespace character_service.Features.Characteristics;

using Microsoft.EntityFrameworkCore;


public class CharacteristicRepository : ICharacteristicRepository
{
    private readonly AppDbContext _context;
    public CharacteristicRepository(AppDbContext context) => _context = context;

    public async Task<Characteristic> CreateAsync(Characteristic characteristic)
    {
        _context.Characteristics.Add(characteristic);
        await _context.SaveChangesAsync();
        return characteristic;
    }

    public async Task<Characteristic?> GetByIdAsync(Guid id, Guid userId)
        => await _context.Characteristics.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

    public async Task<List<Characteristic>> GetByUserAsync(Guid userId)
        => await _context.Characteristics.Where(c => c.UserId == userId).ToListAsync();

    public async Task<Characteristic> UpdateAsync(Characteristic characteristic)
    {
        _context.Characteristics.Update(characteristic);
        await _context.SaveChangesAsync();
        return characteristic;
    }

    public async Task DeleteAsync(Characteristic characteristic)
    {
        _context.Characteristics.Remove(characteristic);
        await _context.SaveChangesAsync();
    }

    public async Task<int> GetTotalLevelsAsync(Guid userId)
        => await _context.Characteristics
            .Where(c => c.UserId == userId)
            .SumAsync(c => c.Level);
}