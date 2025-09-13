using Inventory.Management.Domain.Common;
using Inventory.Management.Domain.Entities;
using Inventory.Management.Domain.Events;
using Inventory.Management.Domain.ValueObjects;
using System.Collections.Concurrent;

namespace Inventory.Management.Domain.Aggregates
{
    /// <summary>
    /// Agregado raiz InventoryItem (por StoreId + Sku).
    /// Mantém invariantes de negócio e publica events internos.
    /// </summary>
    public sealed class InventoryItem
    {
        // Identidade do agregado: Store + Sku
        public StoreId StoreId { get; private set; }
        public Sku Sku { get; private set; }

        // Estado
        public int AvailableQuantity { get; private set; } // quantidade física disponível
        public int ReservedQuantity { get; private set; }  // somatório reservado
        public long Version { get; private set; } // para optimistic concurrency (se usar)
        public DateTime LastUpdatedAt { get; private set; }

        // Reservas associadas (entidades do agregado)
        private readonly List<Reservation> _reservations = new();
        public IReadOnlyCollection<Reservation> Reservations => _reservations.AsReadOnly();

        // Domain events produzidos pelo agregado (poderá ser lido pelo application layer)
        private readonly List<IDomainEvent> _events = new();
        public IReadOnlyCollection<IDomainEvent> Events => _events.AsReadOnly();

        // Construtor
        public InventoryItem(StoreId storeId, Sku sku, int initialAvailable = 0)
        {
            StoreId = storeId ?? throw new ArgumentNullException(nameof(storeId));
            Sku = sku ?? throw new ArgumentNullException(nameof(sku));
            AvailableQuantity = initialAvailable >= 0 ? initialAvailable : throw new DomainException("Available inicial inválido.");
            ReservedQuantity = 0;
            LastUpdatedAt = DateTime.UtcNow;
            Version = 0;
        }

        // Re-hidratação (ORM)
        private InventoryItem() { }

        #region Operações de domínio (comportamento rico)

        public bool ReleaseReservation(string reservationId)
        {
            var reservation = _reservations.FirstOrDefault(r => r.ReservationId == reservationId);
            if (reservation is null)
                return false;

            if (reservation.Status != ReservationStatus.Active)
                return false;

            // Libera a reserva ? devolve quantidade para estoque
            AvailableQuantity += reservation.Quantity;
            ReservedQuantity -= reservation.Quantity;

            reservation.MarkAsReleased();

            LastUpdatedAt = DateTime.UtcNow;

            _events.Add(new StockReleasedEvent(StoreId, Sku, reservation.ReservationId, reservation.Quantity));

            return true;
        }


        /// <summary>
        /// Tenta reservar quantidade para um orderId. Lança DomainException em violação.
        /// Retorna a Reservation criada.
        /// </summary>
        public Reservation Reserve(OrderId orderId, Quantity quantity)
        {
            if (orderId is null) throw new ArgumentNullException(nameof(orderId));
            if (quantity is null) throw new ArgumentNullException(nameof(quantity));

            var availableForReserve = AvailableQuantity - ReservedQuantity;
            if (quantity.Value > availableForReserve)
                throw new InsufficientStockException($"Estoque insuficiente. Disponível para reserva: {availableForReserve}."); //todo tratar retorno 

            var reservation = new Reservation(orderId, quantity);
            _reservations.Add(reservation);
            ReservedQuantity += quantity.Value;
            LastUpdatedAt = DateTime.UtcNow;
            Version++;

            // publicar evento de domínio
            _events.Add(new StockReservedEvent(StoreId, Sku, reservation.ReservationId, orderId, quantity));

            return reservation;
        }

        /// <summary>
        /// Confirma (consome) a reserva. Diminui Available e Reserved.
        /// </summary>
        public void Commit(string reservationId, Quantity quantity = null)
        {
            var r = _reservations.SingleOrDefault(x => x.ReservationId == reservationId)
                ?? throw new ReservationNotFoundException("Reserva não encontrada.");

            if (r.Status != ReservationStatus.Pending) throw new InvalidReservationStateException("Reserva em estado inválido para commit.");

            // se quantity for nulo, usa a quantity da reserva
            var qty = quantity ?? r.Quantity;
            if (qty.Value > r.Quantity.Value) throw new DomainException("Quantidade de commit maior que reservado.");

            if (AvailableQuantity < qty.Value) throw new InsufficientStockException("Estouro de estoque ao commitar.");

            // aplicar alterações
            r.MarkCommitted();
            AvailableQuantity -= qty.Value;
            ReservedQuantity -= qty.Value;
            LastUpdatedAt = DateTime.UtcNow;
            Version++;

            _events.Add(new StockCommittedEvent(StoreId, Sku, reservationId, qty));
        }

        /// <summary>
        /// Libera a reserva (cancelamento) e devolve ao available.
        /// </summary>
        public void Release(string reservationId)
        {
            var r = _reservations.SingleOrDefault(x => x.ReservationId == reservationId)
                ?? throw new ReservationNotFoundException("Reserva não encontrada.");

            if (r.Status != ReservationStatus.Pending) throw new InvalidReservationStateException("Reserva em estado inválido para release.");

            r.MarkReleased();
            ReservedQuantity -= r.Quantity.Value;
            LastUpdatedAt = DateTime.UtcNow;
            Version++;

            _events.Add(new StockReleasedEvent(StoreId, Sku, reservationId, r.Quantity));
        }

        /// <summary>
        /// Reposição de estoque por batch.
        /// </summary>
        //public void Replenish(BatchId batchId, Quantity quantity)
        //{
        //    if (batchId is null) throw new ArgumentNullException(nameof(batchId));
        //    if (quantity is null) throw new ArgumentNullException(nameof(quantity));

        //    AvailableQuantity += quantity.Value;
        //    LastUpdatedAt = DateTime.UtcNow;
        //    Version++;

        //    _events.Add(new StockReplenishedEvent(StoreId, Sku, batchId, quantity));
        //}

        public void Replenish(int quantity, string batchId)
        {
            if (quantity <= 0)
                throw new DomainException("Quantity to replenish must be greater than zero.");

            AvailableQuantity += quantity;
            LastUpdatedAt = DateTime.UtcNow;

            // Opcional: registrar um evento de domínio
            _events.Add(new StockReplenishedEvent(StoreId, Sku, batchId, quantity));
        }

        #endregion

        #region Helpers públicos para integração/infrastructure

        /// <summary>
        /// Limpa eventos produzidos (depois de serem publicados pela camada de aplicação).
        /// </summary>
        public void ClearEvents() => _events.Clear();

        #endregion
    }

    //public class InventoryItem : Entity
    //{
    //    private readonly ConcurrentDictionary<Guid, Reservation> _reservations;

    //    public StoreId StoreId { get; private set; }
    //    public Sku Sku { get; private set; }
    //    public Quantity AvailableQuantity { get; private set; }
    //    public Quantity ReservedQuantity { get; private set; }
    //    public IReadOnlyCollection<Reservation> Reservations => _reservations.Values.ToList().AsReadOnly();

    //    private InventoryItem(StoreId storeId, Sku sku)
    //    {
    //        StoreId = storeId;
    //        Sku = sku;
    //        AvailableQuantity = Quantity.Zero;
    //        ReservedQuantity = Quantity.Zero;
    //        _reservations = new ConcurrentDictionary<Guid, Reservation>();
    //    }

    //    public static InventoryItem Create(StoreId storeId, Sku sku)
    //    {
    //        return new InventoryItem(storeId, sku);
    //    }

    //    public Reservation Reserve(Quantity quantity, string orderId)
    //    {
    //        if (AvailableQuantity.Value < quantity.Value + ReservedQuantity.Value)
    //            throw new DomainException($"Insufficient stock. Available: {AvailableQuantity.Value}, Requested: {quantity.Value}");

    //        var reservation = Reservation.Create(orderId, quantity);
    //        if (!_reservations.TryAdd(reservation.ReservationId, reservation))
    //            throw new DomainException("Failed to create reservation.");

    //        ReservedQuantity = Quantity.Create(ReservedQuantity.Value + quantity.Value);
    //        Update();

    //        return reservation;
    //    }

    //    public void Commit(Guid reservationId, Quantity quantity)
    //    {
    //        if (!_reservations.TryGetValue(reservationId, out var reservation))
    //            throw new DomainException("Reservation not found.");

    //        if (reservation.Quantity.Value != quantity.Value)
    //            throw new DomainException("Commit quantity must match reservation quantity.");

    //        reservation.Commit();
            
    //        AvailableQuantity = AvailableQuantity.Subtract(quantity);
    //        ReservedQuantity = ReservedQuantity.Subtract(quantity);
    //        Update();
    //    }

    //    public void Release(Guid reservationId)
    //    {
    //        if (!_reservations.TryGetValue(reservationId, out var reservation))
    //            throw new DomainException("Reservation not found.");

    //        reservation.Release();
    //        ReservedQuantity = ReservedQuantity.Subtract(reservation.Quantity);
    //        Update();
    //    }

    //    public void Replenish(Quantity quantity, BatchId batchId)
    //    {
    //        // Batch ID pode ser usado para rastreamento ou validação adicional
    //        AvailableQuantity = AvailableQuantity.Add(quantity);
    //        Update();
    //    }

    //    public int GetTotalQuantity() => AvailableQuantity.Value;
    //    public int GetAvailableQuantity() => AvailableQuantity.Value - ReservedQuantity.Value;
    //}
}