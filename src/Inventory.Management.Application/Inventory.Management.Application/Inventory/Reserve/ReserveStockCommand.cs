using Inventory.Management.Application.Abstractions.Messaging;

namespace Inventory.Management.Application.Inventory.Reserve
{
    public record ReserveStockCommand(
        int StoreId, 
        string Sku, 
        int Quantity, 
        string OrderId) : ICommand<ReservationResponse>;
}