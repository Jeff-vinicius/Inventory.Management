namespace Inventory.Management.Application.Inventory.GetAvailability
{
    public record SkuAvailabilityResponse(int StoreId, string Sku, int Available, int Reserved, DateTime LastUpdatedAt);
}
