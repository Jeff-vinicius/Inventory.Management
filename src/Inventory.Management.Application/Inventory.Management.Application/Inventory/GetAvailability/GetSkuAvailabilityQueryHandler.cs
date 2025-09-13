using Inventory.Management.Application.Abstractions.Messaging;
using Inventory.Management.Domain.Errors;
using Inventory.Management.Domain.Interfaces;
using Inventory.Management.Domain.ValueObjects;
using Inventory.Management.SharedKernel;

namespace Inventory.Management.Application.Inventory.GetAvailability
{
    internal sealed class GetSkuAvailabilityQueryHandler(IInventoryRepository repository)
          : IQueryHandler<GetSkuAvailabilityQuery, SkuAvailabilityResponse>
    {
        private readonly IInventoryRepository _repository = repository;

        public async Task<Result<SkuAvailabilityResponse>> Handle(GetSkuAvailabilityQuery query, CancellationToken cancellationToken)
        {
            var inventoryItem = await _repository.GetByStoreAndSkuAsync(
                new StoreId(query.StoreId),
                new Sku(query.Sku),
                cancellationToken
            );

            if (inventoryItem is null)
                return Result.Failure<SkuAvailabilityResponse>(InventoryErrors.NotFound(query.StoreId, query.Sku));

            var response = new SkuAvailabilityResponse(
                query.StoreId,
                query.Sku,
                inventoryItem.AvailableQuantity,
                inventoryItem.ReservedQuantity,
                inventoryItem.LastUpdatedAt
            );

            return response;
        }
    }
}
