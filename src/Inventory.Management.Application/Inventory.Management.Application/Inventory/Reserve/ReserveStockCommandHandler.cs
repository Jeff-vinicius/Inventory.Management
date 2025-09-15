using Inventory.Management.Application.Abstractions.Messaging;
using Inventory.Management.Domain.Entities;
using Inventory.Management.Domain.Errors;
using Inventory.Management.Domain.Interfaces;
using Inventory.Management.Domain.ValueObjects;
using Inventory.Management.SharedKernel;
using Microsoft.Extensions.Logging;

namespace Inventory.Management.Application.Inventory.Reserve
{
    public sealed class ReserveStockCommandHandler(
        IInventoryRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<ReserveStockCommandHandler> logger) : ICommandHandler<ReserveStockCommand, ReservationResponse>
    {
        private readonly IInventoryRepository _repository = repository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<ReserveStockCommandHandler> _logger = logger;

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

                if (!inventoryItem.CanReserveStock(quantity))
                    return Result.Failure<ReservationResponse>(ReservationError.InsufficientStock());

                var reservation = inventoryItem.Reserve(orderId, quantity);

                await _repository.UpdateAsync(inventoryItem, cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);

                var response = new ReservationResponse(
                    reservation.ReservationId,
                    inventoryItem.Version,
                    ReservationStatus.Reserved.ToString()
                );

                return Result.Success(response);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);

                _logger.LogError(ex,
                    "ReserveStock failed for Store {StoreId}, SKU {Sku}, OrderId {OrderId}, Quantity {Quantity}",
                    command.StoreId, command.Sku, command.OrderId, command.Quantity);

                return Result.Failure<ReservationResponse>(InventoryErrors.Unexpected());
            }
        }
    }
}
