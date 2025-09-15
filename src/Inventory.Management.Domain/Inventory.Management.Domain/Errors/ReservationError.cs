using Inventory.Management.SharedKernel;

namespace Inventory.Management.Domain.Errors
{
    public static class ReservationError
    {
        public static Error InsufficientStock() => Error.Conflict(
          "Reservation.Failure",
          "Insufficient available stock to generate reserve.");

        public static Error ReservationInactive(string reservationId) => Error.Conflict(
          "Reservation.Failure",
          $"Reservation {reservationId} inactive.");
    }
}
