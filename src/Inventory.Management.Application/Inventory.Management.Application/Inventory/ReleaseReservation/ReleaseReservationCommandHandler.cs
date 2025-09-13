using Inventory.Management.Application.Abstractions.Messaging;
using Inventory.Management.Application.Inventory.Release;
using Inventory.Management.Domain.Interfaces;
using Inventory.Management.Domain.ValueObjects;
using Inventory.Management.SharedKernel;

namespace Inventory.Management.Application.Inventory.ReleaseReservation
{
    internal sealed class ReleaseReservationCommandHandler : ICommandHandler<ReleaseReservationCommand, bool>
    {
        private readonly IInventoryRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ReleaseReservationCommandHandler(
            IInventoryRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(ReleaseReservationCommand command, CancellationToken cancellationToken)
        {
            var storeId = new StoreId(command.StoreId);
            var sku = new Sku(command.Sku);

            var inventoryItem = await _repository.GetByStoreAndSkuAsync(storeId, sku, cancellationToken);

            //if (inventoryItem is null)
            //    return Result.Failure<bool>($"Inventory item not found for Store {command.StoreId}, SKU {command.Sku}");

            var success = inventoryItem.ReleaseReservation(command.ReservationId);

            //if (!success)
            //    return Result.Failure<bool>($"Reservation {command.ReservationId} not found or already released.");

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(true);
        }
    }
}
