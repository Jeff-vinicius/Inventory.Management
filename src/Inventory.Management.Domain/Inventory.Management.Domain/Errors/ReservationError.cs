using Inventory.Management.SharedKernel;

namespace Inventory.Management.Domain.Errors
{
    public static class ReservationError
    {
        public static Error Failure(string reservationId) => Error.Failure(
           "Reservation.Failure",
           $"Reservation {reservationId} not found, inactive or invalid quantity.");

        public static Error InsufficientStock() => Error.Conflict(
          "Reservation.Failure",
          "Insufficient available stock to generate reserve.");
    }
}
