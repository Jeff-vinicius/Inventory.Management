using Inventory.Management.Domain.Common;
using Inventory.Management.Domain.ValueObjects;

namespace Inventory.Management.Domain.Entities
{
    /// <summary>
    /// Entidade Reservation, pertencente ao agregado InventoryItem.
    /// </summary>
    public sealed class Reservation
    {
        public string ReservationId { get; private set; } = Guid.NewGuid().ToString();
        public OrderId OrderId { get; private set; }
        public Quantity Quantity { get; private set; }
        public ReservationStatus Status { get; private set; } = ReservationStatus.Pending;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        public Reservation(OrderId orderId, Quantity quantity)
        {
            OrderId = orderId ?? throw new ArgumentNullException(nameof(orderId));
            Quantity = quantity ?? throw new ArgumentNullException(nameof(quantity));
        }

        // Para rehidratação (ORM)
        private Reservation() { }

        public void MarkCommitted()
        {
            if (Status != ReservationStatus.Pending) throw new InvalidReservationStateException("Só é possível commitar uma reserva em estado Pending.");
            Status = ReservationStatus.Committed;
        }

        public void MarkAsCommitted()
        {
            if (Status == ReservationStatus.Active)
                Status = ReservationStatus.Committed;
        }


        public void MarkAsReleased()
        {
            if (Status == ReservationStatus.Active)
                Status = ReservationStatus.Released;
        }

        public void MarkReleased()
        {
            if (Status != ReservationStatus.Pending) throw new InvalidReservationStateException("Só é possível liberar uma reserva em estado Pending.");
            Status = ReservationStatus.Released;
        }
    }

    public enum ReservationStatus
    {
        Active,
        Pending,
        Committed,
        Released
    }

    //public class Reservation : Entity
    //{
    //    public Guid ReservationId { get; private set; }
    //    public string OrderId { get; private set; }
    //    public Quantity Quantity { get; private set; }
    //    public ReservationStatus Status { get; private set; }

    //    private Reservation(string orderId, Quantity quantity)
    //    {
    //        ReservationId = Guid.NewGuid();
    //        OrderId = orderId;
    //        Quantity = quantity;
    //        Status = ReservationStatus.Pending;
    //    }

    //    public static Reservation Create(string orderId, Quantity quantity)
    //    {
    //        if (string.IsNullOrWhiteSpace(orderId))
    //            throw new DomainException("Order ID cannot be empty.");

    //        return new Reservation(orderId, quantity);
    //    }

    //    public void Commit()
    //    {
    //        if (Status != ReservationStatus.Pending)
    //            throw new DomainException($"Cannot commit reservation in {Status} status.");

    //        Status = ReservationStatus.Committed;
    //        Update();
    //    }

    //    public void Release()
    //    {
    //        if (Status != ReservationStatus.Pending)
    //            throw new DomainException($"Cannot release reservation in {Status} status.");

    //        Status = ReservationStatus.Released;
    //        Update();
    //    }
    //}

    //public enum ReservationStatus
    //{
    //    Pending,
    //    Committed,
    //    Released
    //}
}