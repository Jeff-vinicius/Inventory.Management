using Inventory.Management.Application.Abstractions.Messaging;
using Inventory.Management.Domain.Interfaces;
using Inventory.Management.Domain.ValueObjects;
using Inventory.Management.SharedKernel;

namespace Inventory.Management.Application.Inventory.Replenish
{
    public class ReplenishStockCommandHandler : ICommandHandler<ReplenishStockCommand, bool>
    {
        private readonly IInventoryRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ReplenishStockCommandHandler(
            IInventoryRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(ReplenishStockCommand command, CancellationToken cancellationToken)
        {
            // Tenta buscar item existente no estoque
            var storeId = new StoreId(command.StoreId);
            var sku = new Sku(command.Sku);

            var inventoryItem = await _repository.GetByStoreAndSkuAsync(storeId, sku, cancellationToken);

            if (inventoryItem is null)
            {
                // Se não existe, cria um novo agregado
                inventoryItem = new Domain.Aggregates.InventoryItem(storeId, sku);
                await _repository.AddAsync(inventoryItem, cancellationToken);
            }

            // Aplica a lógica de negócio do agregado
            inventoryItem.Replenish(command.Quantity, command.BatchId);

            // Persiste transação
            //await _unitOfWork.CommitAsync(cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(true);
        }
    }
}
