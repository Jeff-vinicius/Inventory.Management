using Inventory.Management.Application.Abstractions.Messaging;
using Inventory.Management.Domain.Aggregates;
using Inventory.Management.Domain.Errors;
using Inventory.Management.Domain.Interfaces;
using Inventory.Management.Domain.ValueObjects;
using Inventory.Management.SharedKernel;
using Microsoft.Extensions.Logging;

namespace Inventory.Management.Application.Inventory.Replenish
{
    public sealed class ReplenishStockCommandHandler(
        IInventoryRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<ReplenishStockCommandHandler> logger) : ICommandHandler<ReplenishStockCommand, bool>
    {
        private readonly IInventoryRepository _repository = repository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<ReplenishStockCommandHandler> _logger = logger;

        public async Task<Result<bool>> Handle(ReplenishStockCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = new StoreId(command.StoreId);
                var sku = new Sku(command.Sku);

                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                var inventoryItem = await _repository.GetByStoreAndSkuAsync(storeId, sku, cancellationToken);

                if (inventoryItem is null)
                {
                    inventoryItem = new InventoryItem(storeId, sku);
                    await _repository.AddAsync(inventoryItem, cancellationToken);
                }

                inventoryItem.Replenish(command.Quantity, command.BatchId);

                await _unitOfWork.CommitAsync(cancellationToken);

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);

                _logger.LogError(ex,
                    "ReplenishStock failed for Store {StoreId}, SKU {Sku}, BatchId {BatchId}, Quantity {Quantity}",
                    command.StoreId, command.Sku, command.BatchId, command.Quantity);

                return Result.Failure<bool>(InventoryErrors.Unexpected());
            }
        }
    }
}
