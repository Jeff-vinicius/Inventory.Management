using FluentAssertions;
using Inventory.Management.Application.Inventory.ReleaseReservation;
using Inventory.Management.Domain.Aggregates;
using Inventory.Management.Domain.Errors;
using Inventory.Management.Domain.Interfaces;
using Inventory.Management.Domain.ValueObjects;
using Moq;

namespace Inventory.Management.UnitTests.Application.Inventory.ReleaseReservation
{
    public class ReleaseReservationCommandHandlerTest
    {
        private readonly Mock<IInventoryRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly ReleaseReservationCommandHandler _handler;

        public ReleaseReservationCommandHandlerTest()
        {
            _repositoryMock = new Mock<IInventoryRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new ReleaseReservationCommandHandler(_repositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenInventoryItemNotFound()
        {
            // Arrange
            var command = new ReleaseReservationCommand(1, "SKU-123", "RES-001");
            _repositoryMock
                .Setup(r => r.GetByStoreAndSkuAsync(It.IsAny<StoreId>(), It.IsAny<Sku>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((InventoryItem?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeEquivalentTo(InventoryErrors.NotFound(command.StoreId, command.Sku));

            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenReleaseReservationFails()
        {
            // Arrange
            var command = new ReleaseReservationCommand(1, "SKU-123", "RES-001");
            var inventoryItemMock = new Mock<InventoryItem>(new StoreId(command.StoreId), new Sku(command.Sku), 10, 2, DateTime.UtcNow);

            inventoryItemMock.Setup(i => i.ReleaseReservation(command.ReservationId)).Returns(false);

            _repositoryMock
                .Setup(r => r.GetByStoreAndSkuAsync(It.IsAny<StoreId>(), It.IsAny<Sku>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(inventoryItemMock.Object);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeEquivalentTo(ReservationError.Failure(command.ReservationId));

            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenReleaseReservationSucceeds()
        {
            // Arrange
            var command = new ReleaseReservationCommand(1, "SKU-123", "RES-001");
            var inventoryItemMock = new Mock<InventoryItem>(new StoreId(command.StoreId), new Sku(command.Sku), 10, 2, DateTime.UtcNow);

            inventoryItemMock.Setup(i => i.ReleaseReservation(command.ReservationId)).Returns(true);

            _repositoryMock
                .Setup(r => r.GetByStoreAndSkuAsync(It.IsAny<StoreId>(), It.IsAny<Sku>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(inventoryItemMock.Object);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeTrue();

            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
