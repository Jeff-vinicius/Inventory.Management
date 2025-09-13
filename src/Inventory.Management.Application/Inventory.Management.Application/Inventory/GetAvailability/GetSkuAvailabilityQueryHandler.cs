using Inventory.Management.Application.Abstractions.Messaging;
using Inventory.Management.Domain.Aggregates;
using Inventory.Management.Domain.ValueObjects;
using Inventory.Management.SharedKernel;

namespace Inventory.Management.Application.Inventory.GetAvailability
{
    internal sealed class GetSkuAvailabilityQueryHandler()
        : IQueryHandler<GetSkuAvailabilityQuery, SkuAvailabilityResponse>
    {
        public async Task<Result<SkuAvailabilityResponse>> Handle(GetSkuAvailabilityQuery query, CancellationToken cancellationToken)
        {
            var storeId = StoreId.Create(query.StoreId.ToString());
            var sku = Sku.Create(query.Sku);

            //var inventoryItem = await _repository.GetByStoreAndSkuAsync(
            //    storeId, 
            //    sku, 
            //    cancellationToken);

            //if (inventoryItem is null)
            //    return Result.NotFound("Inventário não encontrado");

            //return Result.Success(new SkuAvailabilityResponse(
            //    query.StoreId,
            //    query.Sku,
            //    inventoryItem.GetAvailableQuantity(),
            //    inventoryItem.ReservedQuantity.Value));

            return Result.Success(new SkuAvailabilityResponse(
                query.StoreId,
                query.Sku,
                1,
                1));
        }
    }
}