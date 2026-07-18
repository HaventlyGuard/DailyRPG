namespace inventory_service.Features.Inventory;

public class UserItem
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ItemId { get; set; }         
    public string ItemName { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty;  
    public int Quantity { get; set; } = 1;
    public bool IsEquipped { get; set; }
    public string? Slot { get; set; }        // Head, Chest, Legs, Weapon, Accessory, Ring
    public string? EffectsJson { get; set; } // Эффекты предмета
    public DateTime AcquiredAt { get; set; }
}