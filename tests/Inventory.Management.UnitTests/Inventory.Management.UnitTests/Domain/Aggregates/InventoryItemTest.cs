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

        [Fact]
        public void Constructor_WithNullStoreId_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            var act = () => new InventoryItem(null!, _validSku, 10);
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("storeId");
        }

        [Fact]
        public void Constructor_WithNegativeInitialQuantity_ShouldThrowDomainException()
        {
            // Arrange, Act & Assert
            var act = () => new InventoryItem(_validStoreId, _validSku, -1);
            act.Should().Throw<DomainException>()
                .WithMessage("Initial quantity invalid!");
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

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Replenish_WithInvalidQuantity_ShouldThrowDomainException(int quantity)
        {
            // Arrange
            var item = new InventoryItem(_validStoreId, _validSku);

            // Act & Assert
            var act = () => item.Replenish(quantity, "BATCH-001");
            act.Should().Throw<DomainException>()
                .WithMessage("Quantity to replenish must be greater than zero.");
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

        [Fact]
        public void Reserve_WithInsufficientStock_ShouldThrowInsufficientStockException()
        {
            // Arrange
            var item = new InventoryItem(_validStoreId, _validSku, 5);
            var orderId = new OrderId("ORDER-001");
            var quantity = new Quantity(10);

            // Act & Assert
            var act = () => item.Reserve(orderId, quantity);
            act.Should().Throw<InsufficientStockException>();
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
            var result = item.CommitReservation(reservation.ReservationId);

            // Assert
            result.Should().BeTrue();
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
            var result = item.CommitReservation("INVALID-ID");

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
            var result = item.ReleaseReservation(reservation.ReservationId);

            // Assert
            result.Should().BeTrue();
            item.AvailableQuantity.Should().Be(10);
            item.ReservedQuantity.Should().Be(0);
            item.Events.Should().Contain(e => e is StockReleasedEvent);
        }

        [Fact]
        public void ReleaseReservation_WithInvalidReservationId_ShouldReturnFalse()
        {
            // Arrange
            var item = new InventoryItem(_validStoreId, _validSku, 10);

            // Act
            var result = item.ReleaseReservation("INVALID-ID");

            // Assert
            result.Should().BeFalse();
        }
        #endregion
    }
}
