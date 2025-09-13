using Inventory.Management.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Management.Domain.Events
{
    public interface IDomainEvent { DateTime OccurredAt { get; } }

    public sealed record StockReservedEvent(StoreId StoreId, Sku Sku, string ReservationId, OrderId OrderId, Quantity Quantity) : IDomainEvent
    {
        public DateTime OccurredAt { get; } = DateTime.UtcNow;
    }

    public sealed record StockCommittedEvent(StoreId StoreId, Sku Sku, string ReservationId, Quantity Quantity) : IDomainEvent
    {
        public DateTime OccurredAt { get; } = DateTime.UtcNow;
    }

    public sealed record StockReleasedEvent(StoreId StoreId, Sku Sku, string ReservationId, Quantity Quantity) : IDomainEvent
    {
        public DateTime OccurredAt { get; } = DateTime.UtcNow;
    }

    public sealed record StockReplenishedEvent(StoreId StoreId, Sku Sku, BatchId BatchId, Quantity Quantity) : IDomainEvent
    {
        public DateTime OccurredAt { get; } = DateTime.UtcNow;
    }
}
