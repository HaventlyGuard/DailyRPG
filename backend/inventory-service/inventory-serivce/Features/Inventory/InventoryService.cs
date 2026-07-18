using inventory_service.Core.DTOs;

namespace inventory_service.Features.Inventory;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _repo;
    private static readonly string[] ValidSlots = { "Head", "Chest", "Legs", "Weapon", "Accessory", "Ring" };

    public InventoryService(IInventoryRepository repo) => _repo = repo;

    public async Task<InventorySummaryDto> GetInventoryAsync(Guid userId)
    {
        var items = await _repo.GetUserItemsAsync(userId);
        var equipped = items.Where(i => i.IsEquipped).ToList();
        return new InventorySummaryDto(
            items.Select(MapToDto).ToList(),
            equipped.Select(MapToDto).ToList(),
            items.Count
        );
    }

    public async Task<List<UserItemDto>> GetEquippedItemsAsync(Guid userId)
    {
        var items = await _repo.GetEquippedItemsAsync(userId);
        return items.Select(MapToDto).ToList();
    }

    public async Task<UserItemDto> EquipItemAsync(Guid userId, Guid itemId, string slot)
    {
        if (!ValidSlots.Contains(slot))
            throw new ArgumentException($"Invalid slot. Valid slots: {string.Join(", ", ValidSlots)}");

        var item = await _repo.GetByIdAsync(itemId, userId);
        if (item == null) throw new KeyNotFoundException("Item not found in inventory");
        if (item.ItemType != "Equipment") throw new InvalidOperationException("Only equipment can be equipped");

        // Снимаем предыдущий предмет с этого слота
        var existingEquipped = await _repo.GetBySlotAsync(userId, slot);
        if (existingEquipped != null)
        {
            existingEquipped.IsEquipped = false;
            existingEquipped.Slot = null;
            await _repo.UpdateItemAsync(existingEquipped);
        }

        // Экипируем новый
        item.IsEquipped = true;
        item.Slot = slot;
        await _repo.UpdateItemAsync(item);

        return MapToDto(item);
    }

    public async Task<UserItemDto> UnequipItemAsync(Guid userId, Guid itemId)
    {
        var item = await _repo.GetByIdAsync(itemId, userId);
        if (item == null) throw new KeyNotFoundException("Item not found in inventory");
        if (!item.IsEquipped) throw new InvalidOperationException("Item is not equipped");

        item.IsEquipped = false;
        item.Slot = null;
        await _repo.UpdateItemAsync(item);

        return MapToDto(item);
    }

    public async Task UseItemAsync(Guid userId, Guid itemId)
    {
        var item = await _repo.GetByIdAsync(itemId, userId);
        if (item == null) throw new KeyNotFoundException("Item not found in inventory");
        if (item.ItemType == "Equipment") throw new InvalidOperationException("Equipment cannot be used, equip it instead");

        await _repo.RemoveItemAsync(item);
        // TODO: Для бустеров - активировать эффект через RabbitMQ
    }

    public async Task AddItemToInventoryAsync(Guid userId, Guid itemId, string itemName, string itemType, string? effectsJson, int quantity = 1)
    {
        // Для Equipment - всегда отдельная запись (не стакается)
        if (itemType == "Equipment")
        {
            var newItem = new UserItem
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ItemId = itemId,
                ItemName = itemName,
                ItemType = itemType,
                Quantity = 1,
                EffectsJson = effectsJson,
                AcquiredAt = DateTime.UtcNow
            };
            await _repo.AddItemAsync(newItem);
        }
        else
        {
            // Для остальных - стакаем если уже есть
            var existingItem = (await _repo.GetUserItemsAsync(userId))
                .FirstOrDefault(i => i.ItemId == itemId && !i.IsEquipped);
            
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                await _repo.UpdateItemAsync(existingItem);
            }
            else
            {
                var newItem = new UserItem
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ItemId = itemId,
                    ItemName = itemName,
                    ItemType = itemType,
                    Quantity = quantity,
                    EffectsJson = effectsJson,
                    AcquiredAt = DateTime.UtcNow
                };
                await _repo.AddItemAsync(newItem);
            }
        }
    }

    private static UserItemDto MapToDto(UserItem item) => new(
        item.Id,
        item.ItemId,
        item.ItemName,
        item.ItemType,
        item.Quantity,
        item.IsEquipped,
        item.Slot,
        item.EffectsJson,
        item.AcquiredAt
    );
}