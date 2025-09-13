using Inventory.Management.SharedKernel;

namespace Inventory.Management.Domain.Errors
{
    public static class InventoryErrors
    {
        public static Error NotFound(int storeId, string skuId) => Error.NotFound(
            "Inventory.NotFound",
            $"Inventory not found for Store {storeId}, SKU {skuId}");
    }
}
