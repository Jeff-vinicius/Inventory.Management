using Inventory.Management.Domain.Aggregates;
using Inventory.Management.Domain.ValueObjects;

namespace Inventory.Management.Domain.Interfaces
{
    public interface IInventoryRepository
    {
        Task<InventoryItem?> GetByStoreAndSkuAsync(StoreId storeId, Sku sku, CancellationToken cancellationToken = default);

        Task AddAsync(InventoryItem item, CancellationToken cancellationToken = default);

        Task UpdateAsync(InventoryItem item, CancellationToken cancellationToken = default);

        /// <summary>
        /// Opcional: método para checar existência e criação atômica (upsert), dependendo da infra.
        /// </summary>
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }

    //public interface IInventoryRepository
    //{
    //Task<InventoryItem?> GetByStoreAndSkuAsync(
    //    StoreId storeId,
    //    Sku sku,
    //    CancellationToken cancellationToken);

    //Task UpdateAsync(
    //    InventoryItem item,
    //    CancellationToken cancellationToken);

    /// <summary>
    /// Contrato de repositório para o agregado InventoryItem.
    /// A infra implementa operações de persistência/transação.
    /// </summary>

    //}
}
