namespace Inventory.Management.API.Models.Inventory
{
    public record CommitRequest(string ReservationId, int Quantity);
}
