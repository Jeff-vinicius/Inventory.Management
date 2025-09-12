namespace Inventory.Management.API.Models.DTOs
{
    public class InventoryAdjustmentRequest
    {
        public int ProductId { get; set; }
        public int NewQuantity { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}