using FluentAssertions;
using Inventory.Management.Domain.Aggregates;
using Inventory.Management.Domain.Common;
using Inventory.Management.Domain.ValueObjects;
using Inventory.Management.Domain.Events;

namespace Inventory.Management.UnitTests.Domain.Aggregates
{
    public class InventoryItemTest
    {
        #region Setup
        private readonly StoreId _validStoreId;
        private readonly Sku _validSku;

        public InventoryItemTest()
        {
            _validStoreId = new StoreId(1);
            _validSku = new Sku("SKU-123");
        }
        #endregion

        #region Constructor Tests
        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateInstance()
        {
            // Arrange & Act
            var item = new InventoryItem(_validStoreId, _validSku, 10);

            // Assert
            item.Should().NotBeNull();
            item.StoreId.Should().Be(_validStoreId);
            item.Sku.Should().Be(_validSku);
            item.AvailableQuantity.Should().Be(10);
            item.ReservedQuantity.Should().Be(0);
            item.Version.Should().Be(0);
            item.LastUpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }
        #endregion

        #region Replenish Tests
        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        public void Replenish_WithValidQuantity_ShouldIncreaseAvailableQuantity(int quantity)
        {
            // Arrange
            var item = new InventoryItem(_validStoreId, _validSku);
            var initialQuantity = item.AvailableQuantity;
            var batchId = "BATCH-001";

            // Act
            item.Replenish(quantity, batchId);

            // Assert
            item.AvailableQuantity.Should().Be(initialQuantity + quantity);
            item.Events.Should().ContainSingle()
                .Which.Should().BeOfType<StockReplenishedEvent>();
        }
        #endregion

        #region Reserve Tests
        [Fact]
        public void Reserve_WithSufficientStock_ShouldCreateReservation()
        {
            // Arrange
            var item = new InventoryItem(_validStoreId, _validSku, 10);
            var orderId = new OrderId("ORDER-001");
            var quantity = new Quantity(5);

            // Act
            var reservation = item.Reserve(orderId, quantity);

            // Assert
            reservation.Should().NotBeNull();
            item.ReservedQuantity.Should().Be(5);
            item.AvailableQuantity.Should().Be(5);
            item.Events.Should().ContainSingle()
                .Which.Should().BeOfType<StockReservedEvent>();
        }
        #endregion

        #region CommitReservation Tests
        [Fact]
        public void CommitReservation_WithValidReservation_ShouldUpdateQuantities()
        {
            // Arrange
            var item = new InventoryItem(_validStoreId, _validSku, 10);
            var orderId = new OrderId("ORDER-001");
            var quantity = new Quantity(5);
            var reservation = item.Reserve(orderId, quantity);

            // Act
            item.CommitReservation(reservation.ReservationId);

            // Assert
            item.AvailableQuantity.Should().Be(5);
            item.ReservedQuantity.Should().Be(0);
            item.Events.Should().Contain(e => e is StockCommittedEvent);
        }

        [Fact]
        public void CommitReservation_WithInvalidReservationId_ShouldReturnFalse()
        {
            // Arrange
            var item = new InventoryItem(_validStoreId, _validSku, 10);

            // Act
            var result = item.HasActiveReservation("INVALID-ID");

            // Assert
            result.Should().BeFalse();
        }
        #endregion

        #region ReleaseReservation Tests
        [Fact]
        public void ReleaseReservation_WithValidReservation_ShouldUpdateQuantities()
        {
            // Arrange
            var item = new InventoryItem(_validStoreId, _validSku, 10);
            var orderId = new OrderId("ORDER-001");
            var quantity = new Quantity(5);
            var reservation = item.Reserve(orderId, quantity);

            // Act
            item.ReleaseReservation(reservation.ReservationId);

            // Assert
            item.AvailableQuantity.Should().Be(10);
            item.ReservedQuantity.Should().Be(0);
            item.Events.Should().Contain(e => e is StockReleasedEvent);
        }
        #endregion
    }
}
