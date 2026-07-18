namespace store_service.Core.DTOs;

public record CreateItemRequest(
    string Name,
    string? Description,
    string Type,        
    string Rarity,       
    int CostGold = 0,
    int CostSilver = 0,
    int CostDiamonds = 0,
    string? EffectsJson = null,
    int? DurationMinutes = null
);

public record UpdateItemRequest(
    string? Name,
    string? Description,
    string? Type,
    string? Rarity,
    int? CostGold,
    int? CostSilver,
    int? CostDiamonds,
    string? EffectsJson,
    int? DurationMinutes,
    bool? IsAvailable
);

public record CatalogItemDto(
    Guid Id,
    string Name,
    string? Description,
    string Type,
    string Rarity,
    int CostGold,
    int CostSilver,
    int CostDiamonds,
    string? EffectsJson,
    int? DurationMinutes,
    bool IsAvailable
);

public record PurchaseRequest(Guid ItemId, string PaymentCurrency); 
public record PurchaseResponse(Guid ItemId, string ItemName, string PaymentCurrency, int Amount);