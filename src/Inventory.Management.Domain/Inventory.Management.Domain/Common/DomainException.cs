namespace Inventory.Management.Domain.Common
{
    public class DomainException : Exception
    {
        public DomainException() { }
        public DomainException(string message) : base(message) { }
        public DomainException(string message, Exception inner) : base(message, inner) { }
    }

    public class InsufficientStockException : DomainException
    {
        public InsufficientStockException(string message) : base(message) { }
    }

    public class ReservationNotFoundException : DomainException
    {
        public ReservationNotFoundException(string message) : base(message) { }
    }

    public class InvalidReservationStateException : DomainException
    {
        public InvalidReservationStateException(string message) : base(message) { }
    }
}