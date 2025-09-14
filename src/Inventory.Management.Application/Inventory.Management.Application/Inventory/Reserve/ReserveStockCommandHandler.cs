using Inventory.Management.Application.Abstractions.Messaging;
using Inventory.Management.Domain.Errors;
using Inventory.Management.Domain.Interfaces;
using Inventory.Management.Domain.ValueObjects;
using Inventory.Management.SharedKernel;

namespace Inventory.Management.Application.Inventory.Reserve
{
    public sealed class ReserveStockCommandHandler(
        IInventoryRepository repository,
        IUnitOfWork unitOfWork) : ICommandHandler<ReserveStockCommand, ReservationResponse>
    {
        private readonly IInventoryRepository _repository = repository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<ReservationResponse>> Handle(ReserveStockCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = new StoreId(command.StoreId);
                var sku = new Sku(command.Sku);

                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                var inventoryItem = await _repository.GetByStoreAndSkuAsync(storeId, sku, cancellationToken);

                if (inventoryItem is null)
                    return Result.Failure<ReservationResponse>(InventoryErrors.NotFound(storeId.Value, sku));

                var orderId = new OrderId(command.OrderId);
                var quantity = new Quantity(command.Quantity);

                var reservation = inventoryItem.Reserve(orderId, quantity);

                await _repository.UpdateAsync(inventoryItem, cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);

                // Publica eventos do agregado, se houver, consistencia eventual, publica depois de commitar a transação
                var events = inventoryItem.Events;
                inventoryItem.ClearEvents();
                // Aqui você chamaria seu EventBus ou Publisher
                // await _eventBus.PublishAsync(events);

                var response = new ReservationResponse(
                    reservation.ReservationId,
                    inventoryItem.Version,
                    "reserved"
                );

                return Result.Success(response);
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
