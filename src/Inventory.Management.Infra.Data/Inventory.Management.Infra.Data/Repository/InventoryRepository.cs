using Microsoft.EntityFrameworkCore;
using Inventory.Management.Domain.Aggregates;
using Inventory.Management.Domain.ValueObjects;
using Inventory.Management.Infra.Data.Context;
using Inventory.Management.Domain.Interfaces;

namespace Inventory.Management.Infra.Data.Repository
{
    public class InventoryRepository(InventoryDbContext context) : IInventoryRepository
    {
        private readonly InventoryDbContext _context = context;

        public async Task<InventoryItem?> GetByStoreAndSkuAsync(StoreId storeId, Sku sku, CancellationToken cancellationToken = default)
        {
            return await _context.InventoryItems
                .Include(i => i.Reservations)
                .SingleOrDefaultAsync(i => i.StoreId == storeId && i.Sku == sku, cancellationToken);
        }

        public async Task AddAsync(InventoryItem item, CancellationToken cancellationToken = default)
        {
            await _context.InventoryItems.AddAsync(item, cancellationToken);
        }

        public Task UpdateAsync(InventoryItem item, CancellationToken cancellationToken = default)
        {
            _context.InventoryItems.Update(item);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}