using Inventory.Management.Domain.Common;
using Inventory.Management.Domain.Entities;
using Inventory.Management.Domain.ValueObjects;
using System.Collections.Concurrent;

namespace Inventory.Management.Domain.Aggregates
{
    public class InventoryItem : Entity
    {
        private readonly ConcurrentDictionary<Guid, Reservation> _reservations;

        public StoreId StoreId { get; private set; }
        public Sku Sku { get; private set; }
        public Quantity AvailableQuantity { get; private set; }
        public Quantity ReservedQuantity { get; private set; }
        public IReadOnlyCollection<Reservation> Reservations => _reservations.Values.ToList().AsReadOnly();

        private InventoryItem(StoreId storeId, Sku sku)
        {
            StoreId = storeId;
            Sku = sku;
            AvailableQuantity = Quantity.Zero;
            ReservedQuantity = Quantity.Zero;
            _reservations = new ConcurrentDictionary<Guid, Reservation>();
        }

        public static InventoryItem Create(StoreId storeId, Sku sku)
        {
            return new InventoryItem(storeId, sku);
        }

        public Reservation Reserve(Quantity quantity, string orderId)
        {
            if (AvailableQuantity.Value < quantity.Value + ReservedQuantity.Value)
                throw new DomainException($"Insufficient stock. Available: {AvailableQuantity.Value}, Requested: {quantity.Value}");

            var reservation = Reservation.Create(orderId, quantity);
            if (!_reservations.TryAdd(reservation.ReservationId, reservation))
                throw new DomainException("Failed to create reservation.");

            ReservedQuantity = Quantity.Create(ReservedQuantity.Value + quantity.Value);
            Update();

            return reservation;
        }

        public void Commit(Guid reservationId, Quantity quantity)
        {
            if (!_reservations.TryGetValue(reservationId, out var reservation))
                throw new DomainException("Reservation not found.");

            if (reservation.Quantity.Value != quantity.Value)
                throw new DomainException("Commit quantity must match reservation quantity.");

            reservation.Commit();
            
            AvailableQuantity = AvailableQuantity.Subtract(quantity);
            ReservedQuantity = ReservedQuantity.Subtract(quantity);
            Update();
        }

        public void Release(Guid reservationId)
        {
            if (!_reservations.TryGetValue(reservationId, out var reservation))
                throw new DomainException("Reservation not found.");

            reservation.Release();
            ReservedQuantity = ReservedQuantity.Subtract(reservation.Quantity);
            Update();
        }

        public void Replenish(Quantity quantity, BatchId batchId)
        {
            // Batch ID pode ser usado para rastreamento ou validação adicional
            AvailableQuantity = AvailableQuantity.Add(quantity);
            Update();
        }

        public int GetTotalQuantity() => AvailableQuantity.Value;
        public int GetAvailableQuantity() => AvailableQuantity.Value - ReservedQuantity.Value;
    }
}