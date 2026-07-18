using Microsoft.EntityFrameworkCore;
using store_service.Core.DataAccess;
using store_service.Core.Enums;

namespace store_service.Features.Store;

public class StoreRepository : IStoreRepository
{
    private readonly AppDbContext _context;
    public StoreRepository(AppDbContext context) => _context = context;

    public async Task<List<CatalogItem>> GetAllAsync(ItemType? type = null)
    {
        var query = _context.CatalogItems.Where(i => i.IsAvailable);
        if (type.HasValue)
            query = query.Where(i => i.Type == type.Value);
        return await query.OrderBy(i => i.Rarity).ThenBy(i => i.Name).ToListAsync();
    }

    public async Task<CatalogItem?> GetByIdAsync(Guid id)
        => await _context.CatalogItems.FirstOrDefaultAsync(i => i.Id == id && i.IsAvailable);

    public async Task<CatalogItem> CreateAsync(CatalogItem item)
    {
        _context.CatalogItems.Add(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task<CatalogItem> UpdateAsync(CatalogItem item)
    {
        _context.CatalogItems.Update(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task DeleteAsync(CatalogItem item)
    {
        item.IsAvailable = false;
        _context.CatalogItems.Update(item);
        await _context.SaveChangesAsync();
    }
}