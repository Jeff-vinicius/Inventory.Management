using FluentAssertions;
using Inventory.Management.Domain.Entities;
using Inventory.Management.Domain.ValueObjects;

namespace Inventory.Management.UnitTests.Domain.Entities
{
    public class ReservationTest
    {
        #region Constructor Tests
        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateInstance()
        {
            // Arrange
            var orderId = new OrderId("ORDER-001");
            var quantity = new Quantity(5);

            // Act
            var reservation = new Reservation(orderId, quantity);

            // Assert
            reservation.Should().NotBeNull();
            reservation.OrderId.Should().Be(orderId);
            reservation.Quantity.Should().Be(quantity);
            reservation.Status.Should().Be(ReservationStatus.Active);
            reservation.ReservationId.Should().NotBeNullOrEmpty();
            reservation.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Constructor_WithNullOrderId_ShouldThrowArgumentNullException()
        {
            // Arrange
            OrderId? orderId = null;
            var quantity = new Quantity(5);

            // Act & Assert
            var act = () => new Reservation(orderId!, quantity);
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("orderId");
        }

        [Fact]
        public void Constructor_WithNullQuantity_ShouldThrowArgumentNullException()
        {
            // Arrange
            var orderId = new OrderId("ORDER-001");
            Quantity? quantity = null;

            // Act & Assert
            var act = () => new Reservation(orderId, quantity!);
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("quantity");
        }
        #endregion

        #region MarkAsCommitted Tests
        [Fact]
        public void MarkAsCommitted_WhenStatusIsActive_ShouldChangeStatusToCommitted()
        {
            // Arrange
            var reservation = new Reservation(new OrderId("ORDER-001"), new Quantity(5));

            // Act
            reservation.MarkAsCommitted();

            // Assert
            reservation.Status.Should().Be(ReservationStatus.Committed);
        }

        [Theory]
        [InlineData(ReservationStatus.Committed)]
        [InlineData(ReservationStatus.Released)]
        [InlineData(ReservationStatus.Pending)]
        public void MarkAsCommitted_WhenStatusIsNotActive_ShouldNotChangeStatus(ReservationStatus initialStatus)
        {
            // Arrange
            var reservation = new Reservation(new OrderId("ORDER-001"), new Quantity(5));
            typeof(Reservation).GetProperty(nameof(Reservation.Status))!
                .SetValue(reservation, initialStatus);

            // Act
            reservation.MarkAsCommitted();

            // Assert
            reservation.Status.Should().Be(initialStatus);
        }
        #endregion

        #region MarkAsReleased Tests
        [Fact]
        public void MarkAsReleased_WhenStatusIsActive_ShouldChangeStatusToReleased()
        {
            // Arrange
            var reservation = new Reservation(new OrderId("ORDER-001"), new Quantity(5));

            // Act
            reservation.MarkAsReleased();

            // Assert
            reservation.Status.Should().Be(ReservationStatus.Released);
        }

        [Theory]
        [InlineData(ReservationStatus.Committed)]
        [InlineData(ReservationStatus.Released)]
        [InlineData(ReservationStatus.Pending)]
        public void MarkAsReleased_WhenStatusIsNotActive_ShouldNotChangeStatus(ReservationStatus initialStatus)
        {
            // Arrange
            var reservation = new Reservation(new OrderId("ORDER-001"), new Quantity(5));
            typeof(Reservation).GetProperty(nameof(Reservation.Status))!
                .SetValue(reservation, initialStatus);

            // Act
            reservation.MarkAsReleased();

            // Assert
            reservation.Status.Should().Be(initialStatus);
        }
        #endregion
    }
}
