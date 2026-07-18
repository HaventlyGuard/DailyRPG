using store_service.Core.Enums;

namespace store_service.Features.Store;

public class CatalogItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ItemType Type { get; set; }
    public ItemRarity Rarity { get; set; } = ItemRarity.Common;
    public int CostGold { get; set; }
    public int CostSilver { get; set; }
    public int CostDiamonds { get; set; }
    public string? EffectsJson { get; set; } 
    public int? DurationMinutes { get; set; } 
    public bool IsAvailable { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}