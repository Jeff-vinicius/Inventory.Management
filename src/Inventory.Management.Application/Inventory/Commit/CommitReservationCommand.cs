using Inventory.Management.Application.Abstractions.Messaging;

namespace Inventory.Management.Application.Inventory.Commit
{
    public record CommitReservationCommand(
        int StoreId, 
        string Sku, 
        string ReservationId, 
        int Quantity) : ICommand<bool>;
}