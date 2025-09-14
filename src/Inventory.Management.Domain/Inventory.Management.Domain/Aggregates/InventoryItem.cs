using Inventory.Management.Domain.Common;
using Inventory.Management.Domain.Entities;
using Inventory.Management.Domain.Events;
using Inventory.Management.Domain.ValueObjects;

namespace Inventory.Management.Domain.Aggregates
{
    public sealed class InventoryItem
    {
        #region Properties and State
        public StoreId StoreId { get; private set; }
        public Sku Sku { get; private set; }

        public int AvailableQuantity { get; private set; }
        public int ReservedQuantity { get; private set; }
        public long Version { get; private set; } // para optimistic concurrency (se usar)
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
            StoreId = storeId ?? throw new ArgumentNullException(nameof(storeId));
            Sku = sku ?? throw new ArgumentNullException(nameof(sku));
            AvailableQuantity = initialAvailable >= 0 ? initialAvailable : throw new DomainException("Initial quantity invalid!");
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
        /// Releases a reservation and returns the quantity back to available stock.
        /// </summary>
        /// <param name="reservationId">The unique identifier of the reservation to be released</param>
        /// <returns>True if the reservation was successfully released, false if the reservation was not found or not in active status</returns>
        public bool ReleaseReservation(string reservationId)
        {
            var reservation = _reservations.FirstOrDefault(r => r.ReservationId == reservationId);
            if (reservation is null)
                return false;

            if (reservation.Status != ReservationStatus.Active)
                return false;

            AvailableQuantity += reservation.Quantity;
            ReservedQuantity -= reservation.Quantity;

            reservation.MarkAsReleased();

            LastUpdatedAt = DateTime.UtcNow;

            _events.Add(new StockReleasedEvent(StoreId, Sku, reservation.ReservationId, reservation.Quantity));

            return true;
        }

        /// <summary>
        /// Creates a new stock reservation for a given order.
        /// </summary>
        /// <param name="orderId">The unique identifier of the order requesting the reservation</param>
        /// <param name="quantity">The quantity to be reserved, must be positive and not exceed available stock</param>
        /// <returns>A new Reservation object representing the created reservation</returns>
        /// <exception cref="ArgumentNullException">Thrown when orderId or quantity is null</exception>
        /// <exception cref="InsufficientStockException">Thrown when there is not enough available stock to fulfill the reservation</exception>
        /// <remarks>
        /// This operation:
        /// - Validates available stock
        /// - Creates and stores a new reservation
        /// - Updates reserved quantity
        /// - Generates a StockReservedEvent
        /// </remarks>
        public Reservation Reserve(OrderId orderId, Quantity quantity)
        {
            if (orderId is null) throw new ArgumentNullException(nameof(orderId));
            if (quantity is null) throw new ArgumentNullException(nameof(quantity));

            var availableForReserve = AvailableQuantity - ReservedQuantity;
            if (quantity.Value > availableForReserve)
                throw new InsufficientStockException($"Insufficient stock available for reservation: {availableForReserve}!");

            var reservation = new Reservation(orderId, quantity);
            _reservations.Add(reservation);
            ReservedQuantity += quantity.Value;
            LastUpdatedAt = DateTime.UtcNow;
            Version++;

            _events.Add(new StockReservedEvent(StoreId, Sku, reservation.ReservationId, orderId, quantity));

            return reservation;
        }

        /// <summary>
        /// Commits an existing reservation by marking it as committed and updating inventory quantities.
        /// </summary>
        /// <param name="reservationId">The unique identifier of the reservation to be committed</param>
        /// <param name="quantity">The quantity to be committed, must match the original reservation quantity</param>
        /// <returns>True if the reservation was successfully committed, false if the reservation was not found, 
        /// not in active status, or quantities don't match</returns>
        /// <remarks>
        /// This operation:
        /// - Validates if reservation exists and is active
        /// - Verifies if the quantity matches the original reservation
        /// - Updates the reservation status to committed
        /// - Decrements the reserved quantity
        /// - Decrements the available quantity
        /// - Generates a StockCommittedEvent
        /// </remarks>
        public bool CommitReservation(string reservationId)
        {
            var reservation = _reservations.FirstOrDefault(r => r.ReservationId == reservationId);
            if (reservation is null)
                return false;

            if (reservation.Status != ReservationStatus.Active)
                return false;

            reservation.MarkAsCommitted();

            ReservedQuantity -= reservation.Quantity;
            AvailableQuantity -= reservation.Quantity;

            LastUpdatedAt = DateTime.UtcNow;

            _events.Add(new StockCommittedEvent(StoreId, Sku, reservation.ReservationId, reservation.Quantity));

            return true;
        }

        /// <summary>
        /// Replenishes the inventory by adding the specified quantity to the available stock.
        /// </summary>
        /// <param name="quantity">The quantity to add to the available stock. Must be greater than zero.</param>
        /// <param name="batchId">The unique identifier of the replenishment batch.</param>
        /// <exception cref="DomainException">Thrown when quantity is zero or negative.</exception>
        /// <remarks>
        /// This operation:
        /// - Validates the quantity is positive
        /// - Increases the available quantity
        /// - Updates the last modified timestamp
        /// - Generates a StockReplenishedEvent
        /// </remarks>
        public void Replenish(int quantity, string batchId)
        {
            if (quantity <= 0)
                throw new DomainException("Quantity to replenish must be greater than zero.");

            AvailableQuantity += quantity;
            LastUpdatedAt = DateTime.UtcNow;

            _events.Add(new StockReplenishedEvent(StoreId, Sku, batchId, quantity));
        }
        #endregion Domain Operations
    }
}