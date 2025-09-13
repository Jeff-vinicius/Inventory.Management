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
            // EF Core já rastreia o agregado; apenas garante que o item esteja sendo monitorado
            _context.InventoryItems.Update(item);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    //public class InventoryRepository : IInventoryRepository
    //{
    //    private readonly InventoryDbContext _context;

    //    public InventoryRepository(InventoryDbContext context)
    //    {
    //        _context = context;
    //    }

    //    public async Task<InventoryItem?> GetByStoreAndSkuAsync(
    //        StoreId storeId,
    //        Sku sku,
    //        CancellationToken cancellationToken)
    //    {
    //        return await _context.InventoryItems
    //            .Include(i => i.Reservations)
    //            .FirstOrDefaultAsync(i => 
    //                i.StoreId.Value == storeId.Value && 
    //                i.Sku.Value == sku.Value, 
    //                cancellationToken);
    //    }

    //    public async Task UpdateAsync(
    //        InventoryItem item,
    //        CancellationToken cancellationToken)
    //    {
    //        _context.Update(item);
    //        await Task.CompletedTask;
    //    }
    //}
}