using inventory_service.Core.DTOs;

namespace inventory_service.Features.Inventory;

public interface IInventoryService
{
    Task<InventorySummaryDto> GetInventoryAsync(Guid userId);
    Task<List<UserItemDto>> GetEquippedItemsAsync(Guid userId);
    Task<UserItemDto> EquipItemAsync(Guid userId, Guid itemId, string slot);
    Task<UserItemDto> UnequipItemAsync(Guid userId, Guid itemId);
    Task UseItemAsync(Guid userId, Guid itemId);
    // Вызывается через RabbitMQ при покупке
    Task AddItemToInventoryAsync(Guid userId, Guid itemId, string itemName, string itemType, string? effectsJson, int quantity = 1);
}