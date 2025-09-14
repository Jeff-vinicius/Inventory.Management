using Inventory.Management.Domain.ValueObjects;

namespace Inventory.Management.Domain.Entities
{
    public sealed class Reservation
    {
        #region Properties and State
        public string ReservationId { get; private set; } = Guid.NewGuid().ToString();
        public OrderId OrderId { get; private set; }
        public Quantity Quantity { get; private set; }
        public ReservationStatus Status { get; private set; } = ReservationStatus.Active;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        #endregion Properties and State

        #region Constructor 
        public Reservation(OrderId orderId, Quantity quantity)
        {
            OrderId = orderId ?? throw new ArgumentNullException(nameof(orderId));
            Quantity = quantity ?? throw new ArgumentNullException(nameof(quantity));
        }
        #endregion Constructor

        #region ORM Constructor
        private Reservation() { }
        #endregion ORM Constructor

        #region Domain Operations
        /// <summary>
        /// Marks the reservation as committed if it is in Active status.
        /// The operation will be ignored if the reservation is not Active.
        /// </summary>
        public void MarkAsCommitted()
        {
            if (Status == ReservationStatus.Active)
                Status = ReservationStatus.Committed;
        }

        /// <summary>
        /// Marks the reservation as released if it is in Active status.
        /// The operation will be ignored if the reservation is not Active.
        /// </summary>
        public void MarkAsReleased()
        {
            if (Status == ReservationStatus.Active)
                Status = ReservationStatus.Released;
        }
        #endregion Domain Operations
    }

    public enum ReservationStatus
    {
        Active,
        Pending,
        Committed,
        Released
    }
}