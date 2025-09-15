using Inventory.Management.Application.Abstractions.Messaging;
using Inventory.Management.Domain.Errors;
using Inventory.Management.Domain.Interfaces;
using Inventory.Management.Domain.ValueObjects;
using Inventory.Management.SharedKernel;
using Microsoft.Extensions.Logging;

namespace Inventory.Management.Application.Inventory.ReleaseReservation
{
    public sealed class ReleaseReservationCommandHandler(
        IInventoryRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<ReleaseReservationCommandHandler> logger) : ICommandHandler<ReleaseReservationCommand, bool>
    {
        private readonly IInventoryRepository _repository = repository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<ReleaseReservationCommandHandler> _logger = logger;

        public async Task<Result<bool>> Handle(ReleaseReservationCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = new StoreId(command.StoreId);
                var sku = new Sku(command.Sku);

                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                var inventoryItem = await _repository.GetByStoreAndSkuAsync(storeId, sku, cancellationToken);

                if (inventoryItem is null)
                    return Result.Failure<bool>(InventoryErrors.NotFound(storeId.Value, sku));

                if (!inventoryItem.HasActiveReservation(command.ReservationId))
                    return Result.Failure<bool>(ReservationError.ReservationInactive(command.ReservationId));

                inventoryItem.ReleaseReservation(command.ReservationId);

                await _unitOfWork.CommitAsync(cancellationToken);

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);

                _logger.LogError(ex,
                    "ReleaseReservation failed for Store {StoreId}, SKU {Sku}, ReservationId {ReservationId}",
                    command.StoreId, command.Sku, command.ReservationId);

                return Result.Failure<bool>(InventoryErrors.Unexpected());
            }
        }
    }
}
