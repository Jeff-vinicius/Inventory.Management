using Inventory.Management.Application.Abstractions.Messaging;
using Inventory.Management.Domain.Errors;
using Inventory.Management.Domain.Interfaces;
using Inventory.Management.Domain.ValueObjects;
using Inventory.Management.SharedKernel;

namespace Inventory.Management.Application.Inventory.Commit
{
    public sealed class CommitReservationCommandHandler(
        IInventoryRepository repository,
        IUnitOfWork unitOfWork) : ICommandHandler<CommitReservationCommand, bool>
    {
        private readonly IInventoryRepository _repository = repository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<bool>> Handle(CommitReservationCommand command, CancellationToken cancellationToken)
        {
            var storeId = new StoreId(command.StoreId);
            var sku = new Sku(command.Sku);

            var inventoryItem = await _repository.GetByStoreAndSkuAsync(storeId, sku, cancellationToken);

            if (inventoryItem is null)
                return Result.Failure<bool>(InventoryErrors.NotFound(storeId.Value, sku));

            var success = inventoryItem.CommitReservation(command.ReservationId);

            if (!success)
                return Result.Failure<bool>(ReservationError.Failure(command.ReservationId));

            await _repository.UpdateAsync(inventoryItem, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(true);
        }
    }
}
