namespace Inventory.Management.API.Models.DTOs
{
    public class InventoryTransferRequest
    {
        public int FromStoreId { get; set; }
        public int ToStoreId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}