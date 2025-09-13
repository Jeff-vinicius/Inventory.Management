using Inventory.Management.Domain.Common;
using Inventory.Management.Domain.ValueObjects;

namespace Inventory.Management.Domain.Entities
{
    public class Reservation : Entity
    {
        public Guid ReservationId { get; private set; }
        public string OrderId { get; private set; }
        public Quantity Quantity { get; private set; }
        public ReservationStatus Status { get; private set; }

        private Reservation(string orderId, Quantity quantity)
        {
            ReservationId = Guid.NewGuid();
            OrderId = orderId;
            Quantity = quantity;
            Status = ReservationStatus.Pending;
        }

        public static Reservation Create(string orderId, Quantity quantity)
        {
            if (string.IsNullOrWhiteSpace(orderId))
                throw new DomainException("Order ID cannot be empty.");

            return new Reservation(orderId, quantity);
        }

        public void Commit()
        {
            if (Status != ReservationStatus.Pending)
                throw new DomainException($"Cannot commit reservation in {Status} status.");

            Status = ReservationStatus.Committed;
            Update();
        }

        public void Release()
        {
            if (Status != ReservationStatus.Pending)
                throw new DomainException($"Cannot release reservation in {Status} status.");

            Status = ReservationStatus.Released;
            Update();
        }
    }

    public enum ReservationStatus
    {
        Pending,
        Committed,
        Released
    }
}