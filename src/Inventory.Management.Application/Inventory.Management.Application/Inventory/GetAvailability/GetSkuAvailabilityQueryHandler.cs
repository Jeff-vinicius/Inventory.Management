using Inventory.Management.Application.Abstractions.Messaging;
using Inventory.Management.Domain.Aggregates;
using Inventory.Management.Domain.Interfaces;
using Inventory.Management.Domain.ValueObjects;
using Inventory.Management.SharedKernel;

namespace Inventory.Management.Application.Inventory.GetAvailability
{
    internal sealed class GetSkuAvailabilityQueryHandler
     : IQueryHandler<GetSkuAvailabilityQuery, SkuAvailabilityResponse>
    {
        private readonly IInventoryRepository _repository;

        public GetSkuAvailabilityQueryHandler(IInventoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<SkuAvailabilityResponse>> Handle(GetSkuAvailabilityQuery query, CancellationToken cancellationToken)
        {
            var result = await _repository.GetByStoreAndSkuAsync(
                new StoreId(query.StoreId),
                new Sku(query.Sku),
                cancellationToken
            );

            //if (result is null)
            //    return Result.NotFound<SkuAvailabilityResponse>($"Item {query.Sku} not found for store {query.StoreId}");

            var response = new SkuAvailabilityResponse(
                query.StoreId,
                query.Sku,
                result.AvailableQuantity,
                result.ReservedQuantity,
                result.LastUpdatedAt
            );

            return response;
        }
    }
}
