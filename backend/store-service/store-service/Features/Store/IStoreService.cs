using store_service.Core.DTOs;

namespace store_service.Features.Store;

public interface IStoreService
{
    Task<List<CatalogItemDto>> GetCatalogAsync(string? type = null);
    Task<CatalogItemDto> GetItemAsync(Guid id);
    Task<CatalogItemDto> CreateItemAsync(CreateItemRequest request);
    Task<CatalogItemDto> UpdateItemAsync(Guid id, UpdateItemRequest request);
    Task DeleteItemAsync(Guid id);
    Task<PurchaseResponse> PurchaseItemAsync(Guid userId, PurchaseRequest request);
}