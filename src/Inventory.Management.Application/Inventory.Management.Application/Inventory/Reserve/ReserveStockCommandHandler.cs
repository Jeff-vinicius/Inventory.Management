using Inventory.Management.Application.Abstractions.Messaging;
using Inventory.Management.Domain.Entities;
using Inventory.Management.Domain.ValueObjects;
using Inventory.Management.SharedKernel;

namespace Inventory.Management.Application.Inventory.Reserve
{
    internal sealed class ReserveStockCommandHandler() : ICommandHandler<ReserveStockCommand, ReservationResponse>
    {
        public async Task<Result<ReservationResponse>> Handle(
            ReserveStockCommand command, 
            CancellationToken cancellationToken)
        {
            
            //var storeId = StoreId.Create(command.StoreId.ToString());
            //var sku = Sku.Create(command.Sku);
            //var quantity = Quantity.Create(command.Quantity);

            //var inventoryItem = await _repository.GetByStoreAndSkuAsync(
            //    storeId, 
            //    sku, 
            //    cancellationToken);

            //if (inventoryItem is null)
            //    return Result.NotFound("Inventário não encontrado");

            //var reservation = inventoryItem.Reserve(quantity, command.OrderId);
                
            //await _repository.UpdateAsync(inventoryItem, cancellationToken);
            //await _unitOfWork.SaveChangesAsync(cancellationToken);

            //return Result.Success(new ReservationResponse(
            //    true, 
            //    reservation.ReservationId.ToString()));

            return Result.Success(new ReservationResponse(
                true,
                "1"));
           
        }
    }
}