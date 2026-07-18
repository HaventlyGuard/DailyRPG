using Microsoft.EntityFrameworkCore;
using inventory_service.Core.DataAccess;

namespace inventory_service.Features.Inventory;

public class InventoryRepository : IInventoryRepository
{
    private readonly AppDbContext _context;
    public InventoryRepository(AppDbContext context) => _context = context;

    public async Task<List<UserItem>> GetUserItemsAsync(Guid userId)
        => await _context.UserItems
            .Where(i => i.UserId == userId && i.Quantity > 0)
            .OrderByDescending(i => i.AcquiredAt)
            .ToListAsync();

    public async Task<List<UserItem>> GetEquippedItemsAsync(Guid userId)
        => await _context.UserItems
            .Where(i => i.UserId == userId && i.IsEquipped)
            .ToListAsync();

    public async Task<UserItem?> GetByIdAsync(Guid id, Guid userId)
        => await _context.UserItems.FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);

    public async Task<UserItem> AddItemAsync(UserItem item)
    {
        _context.UserItems.Add(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task<UserItem> UpdateItemAsync(UserItem item)
    {
        _context.UserItems.Update(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task RemoveItemAsync(UserItem item)
    {
        if (item.Quantity > 1)
        {
            item.Quantity--;
            await UpdateItemAsync(item);
        }
        else
        {
            _context.UserItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<UserItem?> GetBySlotAsync(Guid userId, string slot)
        => await _context.UserItems
            .FirstOrDefaultAsync(i => i.UserId == userId && i.Slot == slot && i.IsEquipped);
}