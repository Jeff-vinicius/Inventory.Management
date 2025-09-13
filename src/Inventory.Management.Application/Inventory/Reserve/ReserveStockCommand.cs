using Inventory.Management.Application.Abstractions.Messaging;
using Inventory.Management.SharedKernel;

namespace Inventory.Management.Application.Inventory.Reserve
{
    public record ReserveStockCommand(
        string StoreId, 
        string Sku, 
        int Quantity, 
        string OrderId) : ICommand<ReservationResponse>;
}