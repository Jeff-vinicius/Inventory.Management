using Inventory.Management.Application.Abstractions.Messaging;
using Inventory.Management.Domain.Aggregates;
using Inventory.Management.Domain.Interfaces;
using Inventory.Management.Domain.ValueObjects;
using Inventory.Management.SharedKernel;

namespace Inventory.Management.Application.Inventory.Replenish
{
    public sealed class ReplenishStockCommandHandler(
        IInventoryRepository repository,
        IUnitOfWork unitOfWork) : ICommandHandler<ReplenishStockCommand, bool>
    {
        private readonly IInventoryRepository _repository = repository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<bool>> Handle(ReplenishStockCommand command, CancellationToken cancellationToken)
        {
            var storeId = new StoreId(command.StoreId);
            var sku = new Sku(command.Sku);

            var inventoryItem = await _repository.GetByStoreAndSkuAsync(storeId, sku, cancellationToken);

            if (inventoryItem is null)
            {
                inventoryItem = new InventoryItem(storeId, sku);
                await _repository.AddAsync(inventoryItem, cancellationToken);
            }

            inventoryItem.Replenish(command.Quantity, command.BatchId);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(true);
        }
    }
}
