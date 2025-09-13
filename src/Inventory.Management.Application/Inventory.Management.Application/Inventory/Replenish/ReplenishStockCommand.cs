using Inventory.Management.Application.Abstractions.Messaging;

namespace Inventory.Management.Application.Inventory.Replenish
{
    public record ReplenishStockCommand(
        int StoreId, 
        string Sku, 
        int Quantity, 
        string BatchId) : ICommand<bool>;
}