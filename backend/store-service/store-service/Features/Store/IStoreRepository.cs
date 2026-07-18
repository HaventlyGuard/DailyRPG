using store_service.Core.Enums;

namespace store_service.Features.Store;

public interface IStoreRepository
{
    Task<List<CatalogItem>> GetAllAsync(ItemType? type = null);
    Task<CatalogItem?> GetByIdAsync(Guid id);
    Task<CatalogItem> CreateAsync(CatalogItem item);
    Task<CatalogItem> UpdateAsync(CatalogItem item);
    Task DeleteAsync(CatalogItem item);
}