namespace inventory_service.Features.Inventory;

public interface IInventoryRepository
{
    Task<List<UserItem>> GetUserItemsAsync(Guid userId);
    Task<List<UserItem>> GetEquippedItemsAsync(Guid userId);
    Task<UserItem?> GetByIdAsync(Guid id, Guid userId);
    Task<UserItem> AddItemAsync(UserItem item);
    Task<UserItem> UpdateItemAsync(UserItem item);
    Task RemoveItemAsync(UserItem item);
    Task<UserItem?> GetBySlotAsync(Guid userId, string slot);
}