using Inventory.Management.Application.Abstractions.Messaging;

namespace Inventory.Management.Application.Inventory.Release
{
    public record ReleaseReservationCommand(
        int StoreId, 
        string Sku, 
        string ReservationId) : ICommand<bool>;
}