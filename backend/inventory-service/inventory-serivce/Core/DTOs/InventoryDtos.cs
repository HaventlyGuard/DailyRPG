namespace inventory_service.Core.DTOs;

public record UserItemDto(
    Guid Id,
    Guid ItemId,
    string ItemName,
    string ItemType,
    int Quantity,
    bool IsEquipped,
    string? Slot,
    string? EffectsJson,
    DateTime AcquiredAt
);

public record EquipRequest(string Slot);
public record InventorySummaryDto(
    List<UserItemDto> Items,
    List<UserItemDto> EquippedItems,
    int TotalItems
);