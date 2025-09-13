using Microsoft.EntityFrameworkCore;
using Inventory.Management.Domain.Aggregates;
using Inventory.Management.Domain.ValueObjects;
using Inventory.Management.Infra.Data.Context;
using Inventory.Management.Domain.Interfaces;

namespace Inventory.Management.Infra.Data.Repository
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly InventoryDbContext _context;

        public InventoryRepository(InventoryDbContext context)
        {
            _context = context;
        }

        public async Task<InventoryItem?> GetByStoreAndSkuAsync(
            StoreId storeId,
            Sku sku,
            CancellationToken cancellationToken)
        {
            return await _context.InventoryItems
                .Include(i => i.Reservations)
                .FirstOrDefaultAsync(i => 
                    i.StoreId.Value == storeId.Value && 
                    i.Sku.Value == sku.Value, 
                    cancellationToken);
        }

        public async Task UpdateAsync(
            InventoryItem item,
            CancellationToken cancellationToken)
        {
            _context.Update(item);
            await Task.CompletedTask;
        }
    }
}