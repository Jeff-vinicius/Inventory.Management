using Inventory.Management.API.Models.Enums;

namespace Inventory.Management.API.Models.DTOs
{
    public class InventoryMovementRequest
    {
        public int ProductId { get; set; }
        public InventoryMovementType Type { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string ReferenceId { get; set; } = string.Empty;
    }
}