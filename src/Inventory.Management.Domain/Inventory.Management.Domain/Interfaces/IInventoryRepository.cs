using Inventory.Management.Domain.Aggregates;
using Inventory.Management.Domain.ValueObjects;

namespace Inventory.Management.Domain.Interfaces
{
    public interface IInventoryRepository
    {
        Task<InventoryItem?> GetByStoreAndSkuAsync(StoreId storeId, Sku sku, CancellationToken cancellationToken = default);

        Task AddAsync(InventoryItem item, CancellationToken cancellationToken = default);

        Task UpdateAsync(InventoryItem item, CancellationToken cancellationToken = default);
    }
}
