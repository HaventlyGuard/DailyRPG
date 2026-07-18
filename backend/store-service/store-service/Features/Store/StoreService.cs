using store_service.Core.DTOs;
using store_service.Core.Enums;

namespace store_service.Features.Store;

public class StoreService : IStoreService
{
    private readonly IStoreRepository _repo;
    
    public StoreService(IStoreRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<CatalogItemDto>> GetCatalogAsync(string? type = null)
    {
        ItemType? itemType = null;
        if (!string.IsNullOrEmpty(type))
            itemType = Enum.Parse<ItemType>(type, true);

        var items = await _repo.GetAllAsync(itemType);
        return items.Select(MapToDto).ToList();
    }

    public async Task<CatalogItemDto> GetItemAsync(Guid id)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item == null) throw new KeyNotFoundException("Item not found");
        return MapToDto(item);
    }

    public async Task<CatalogItemDto> CreateItemAsync(CreateItemRequest request)
    {
        var item = new CatalogItem
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Type = Enum.Parse<ItemType>(request.Type, true),
            Rarity = Enum.Parse<ItemRarity>(request.Rarity, true),
            CostGold = request.CostGold,
            CostSilver = request.CostSilver,
            CostDiamonds = request.CostDiamonds,
            EffectsJson = request.EffectsJson,
            DurationMinutes = request.DurationMinutes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repo.CreateAsync(item);
        return MapToDto(item);
    }

    public async Task<CatalogItemDto> UpdateItemAsync(Guid id, UpdateItemRequest request)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item == null) throw new KeyNotFoundException("Item not found");

        if (request.Name != null) item.Name = request.Name;
        if (request.Description != null) item.Description = request.Description;
        if (request.Type != null) item.Type = Enum.Parse<ItemType>(request.Type, true);
        if (request.Rarity != null) item.Rarity = Enum.Parse<ItemRarity>(request.Rarity, true);
        if (request.CostGold.HasValue) item.CostGold = request.CostGold.Value;
        if (request.CostSilver.HasValue) item.CostSilver = request.CostSilver.Value;
        if (request.CostDiamonds.HasValue) item.CostDiamonds = request.CostDiamonds.Value;
        if (request.EffectsJson != null) item.EffectsJson = request.EffectsJson;
        if (request.DurationMinutes.HasValue) item.DurationMinutes = request.DurationMinutes;
        if (request.IsAvailable.HasValue) item.IsAvailable = request.IsAvailable.Value;
        
        item.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(item);
        return MapToDto(item);
    }

    public async Task DeleteItemAsync(Guid id)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item == null) throw new KeyNotFoundException("Item not found");
        await _repo.DeleteAsync(item);
    }

    public async Task<PurchaseResponse> PurchaseItemAsync(Guid userId, PurchaseRequest request)
    {
        var item = await _repo.GetByIdAsync(request.ItemId);
        if (item == null) throw new KeyNotFoundException("Item not found");

        var paymentCurrency = request.PaymentCurrency;
        var cost = GetCost(item, paymentCurrency);
        if (cost <= 0)
            throw new InvalidOperationException($"Item cannot be purchased with {paymentCurrency}");

        // TODO: Проверить баланс через Wallet Service и списать валюту
        // TODO: Опубликовать событие ItemPurchased в RabbitMQ

        return new PurchaseResponse(item.Id, item.Name, paymentCurrency, cost);
    }

    private int GetCost(CatalogItem item, string currency)
    {
        return currency switch
        {
            "Gold" => item.CostGold,
            "Silver" => item.CostSilver,
            "Diamonds" => item.CostDiamonds,
            _ => 0
        };
    }

    private static CatalogItemDto MapToDto(CatalogItem item) => new(
        item.Id,
        item.Name,
        item.Description,
        item.Type.ToString(),
        item.Rarity.ToString(),
        item.CostGold,
        item.CostSilver,
        item.CostDiamonds,
        item.EffectsJson,
        item.DurationMinutes,
        item.IsAvailable
    );
}