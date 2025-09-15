using Inventory.Management.Domain.Common;
using Inventory.Management.Domain.Entities;
using Inventory.Management.Domain.Events;
using Inventory.Management.Domain.ValueObjects;

namespace Inventory.Management.Domain.Aggregates
{
    public class InventoryItem
    {
        #region Properties and State
        public StoreId StoreId { get; private set; }
        public Sku Sku { get; private set; }

        public int AvailableQuantity { get; private set; }
        public int ReservedQuantity { get; private set; }
        public long Version { get; private set; }
        public DateTime LastUpdatedAt { get; private set; }

        private readonly List<Reservation> _reservations = new();
        public IReadOnlyCollection<Reservation> Reservations => _reservations.AsReadOnly();
        #endregion Properties and State

        #region Domain Events
        private readonly List<IDomainEvent> _events = new();
        public IReadOnlyCollection<IDomainEvent> Events => _events.AsReadOnly();
        public void ClearEvents() => _events.Clear();
        #endregion Domain Events

        #region Construtor
        public InventoryItem(StoreId storeId, Sku sku, int initialAvailable = 0)
        {
            StoreId = storeId;
            Sku = sku;
            AvailableQuantity = initialAvailable;
            ReservedQuantity = 0;
            LastUpdatedAt = DateTime.UtcNow;
            Version = 0;
        }
        #endregion Construtor

        #region ORM Constructor
        private InventoryItem() { }
        #endregion ORM Constructor

        #region Domain Operations
        /// <summary>
        /// Releases the specified reservation by updating stock quantities, 
        /// marking the reservation as released, setting the last update timestamp, 
        /// and recording a <see cref="StockReleasedEvent"/>.
        /// </summary>
        public virtual void ReleaseReservation(string reservationId)
        {
            var reservation = _reservations.FirstOrDefault(r => r.ReservationId == reservationId);

            AvailableQuantity += reservation!.Quantity;
            ReservedQuantity -= reservation.Quantity;

            reservation.MarkAsReleased();

            LastUpdatedAt = DateTime.UtcNow;

            _events.Add(new StockReleasedEvent(StoreId, Sku, reservation.ReservationId, reservation.Quantity));
        }

        /// <summary>
        /// Creates and registers a new reservation for the specified order and quantity.
        /// Updates reserved and available stock, increments the version, 
        /// sets the last update timestamp, and records a <see cref="StockReservedEvent"/>.
        /// </summary>
        public virtual Reservation Reserve(OrderId orderId, Quantity quantity)
        {
            var reservation = new Reservation(orderId, quantity);
            _reservations.Add(reservation);
            ReservedQuantity += quantity.Value;
            AvailableQuantity -= quantity.Value;
            LastUpdatedAt = DateTime.UtcNow;
            Version++;

            _events.Add(new StockReservedEvent(StoreId, Sku, reservation.ReservationId, orderId, quantity));

            return reservation;
        }

        /// <summary>
        /// Determines whether the specified quantity can be reserved 
        /// based on the available stock minus the already reserved amount.
        /// Returns <c>true</c> if there is sufficient stock to fulfill the reservation; otherwise, <c>false</c>.
        /// </summary>
        public virtual bool CanReserveStock(Quantity quantity) 
            =>  quantity.Value <= (AvailableQuantity - ReservedQuantity);

        /// <summary>
        /// Commits the specified reservation by marking it as committed, 
        /// updating reserved stock quantities, setting the last update timestamp, 
        /// and recording a <see cref="StockCommittedEvent"/>.
        /// </summary>
        public virtual void CommitReservation(string reservationId)
        {
            var reservation = _reservations.FirstOrDefault(r => r.ReservationId == reservationId);

            reservation!.MarkAsCommitted();

            ReservedQuantity -= reservation.Quantity;

            LastUpdatedAt = DateTime.UtcNow;

            _events.Add(new StockCommittedEvent(StoreId, Sku, reservation.ReservationId, reservation.Quantity));
        }

        /// <summary>
        /// Determines whether a reservation with the specified identifier exists 
        /// and is currently in the <see cref="ReservationStatus.Active"/> state.
        /// Returns <c>true</c> if such a reservation exists; otherwise, <c>false</c>.
        /// </summary>
        public virtual bool HasActiveReservation(string reservationId)
        {
            var reservation = _reservations.FirstOrDefault(r => r.ReservationId == reservationId);
            return reservation is not null && reservation.Status == ReservationStatus.Active;
        }

        /// <summary>
        /// Increases the available stock by the specified quantity, 
        /// updates the last update timestamp, 
        /// and records a <see cref="StockReplenishedEvent"/> for the given batch.
        /// </summary>
        public virtual void Replenish(int quantity, string batchId)
        {
            AvailableQuantity += quantity;
            LastUpdatedAt = DateTime.UtcNow;

            _events.Add(new StockReplenishedEvent(StoreId, Sku, batchId, quantity));
        }
        #endregion Domain Operations
    }
}