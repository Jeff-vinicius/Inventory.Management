namespace Inventory.Management.API.Models.DTOs
{
    public class InventoryAdjustmentResponse
    {
        public int ProductId { get; set; }
        public int PreviousQuantity { get; set; }
        public int NewQuantity { get; set; }
    }
}