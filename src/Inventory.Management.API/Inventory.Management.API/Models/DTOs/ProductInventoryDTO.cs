namespace Inventory.Management.API.Models.DTOs
{
    public class ProductInventoryDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}