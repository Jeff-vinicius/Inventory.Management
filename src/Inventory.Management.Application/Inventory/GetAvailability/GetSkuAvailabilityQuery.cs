using Inventory.Management.Application.Abstractions.Messaging;

namespace Inventory.Management.Application.Inventory.GetAvailability
{
    public sealed record GetSkuAvailabilityQuery(int StoreId, string Sku) : IQuery<SkuAvailabilityResponse>;
}