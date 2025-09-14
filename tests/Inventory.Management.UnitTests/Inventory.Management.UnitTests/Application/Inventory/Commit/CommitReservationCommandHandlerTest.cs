using FluentAssertions;
using Inventory.Management.Application.Inventory.Commit;
using Inventory.Management.Domain.Aggregates;
using Inventory.Management.Domain.Interfaces;
using Inventory.Management.Domain.ValueObjects;
using Moq;

namespace Inventory.Management.UnitTests.Application.Inventory.Commit
{
    public class CommitReservationCommandHandlerTest
    {
        private readonly Mock<IInventoryRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CommitReservationCommandHandler _handler;

        public CommitReservationCommandHandlerTest()
        {
            _repositoryMock = new Mock<IInventoryRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new CommitReservationCommandHandler(_repositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Return_Success_When_Commit_Is_Valid()
        {
            // Arrange
            var storeId = 1;
            var sku = "SKU-123";
            var reservationId = "RES-001";
            var command = new CommitReservationCommand(storeId, sku, reservationId);

            var inventoryItemMock = new Mock<InventoryItem>(new StoreId(storeId), new Sku(sku), 10);
            inventoryItemMock.Setup(i => i.CommitReservation(reservationId)).Returns(true);

            _repositoryMock.Setup(r => r.GetByStoreAndSkuAsync(It.IsAny<StoreId>(), It.IsAny<Sku>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(inventoryItemMock.Object);

            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<InventoryItem>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_When_InventoryItem_Not_Found()
        {
            // Arrange
            var command = new CommitReservationCommand(1, "SKU-123", "RES-001");

            _repositoryMock.Setup(r => r.GetByStoreAndSkuAsync(It.IsAny<StoreId>(), It.IsAny<Sku>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((InventoryItem?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_When_CommitReservation_Returns_False()
        {
            // Arrange
            var storeId = 1;
            var sku = "SKU-123";
            var reservationId = "RES-001";
            var command = new CommitReservationCommand(storeId, sku, reservationId);

            var inventoryItemMock = new Mock<InventoryItem>(new StoreId(storeId), new Sku(sku), 10);
            inventoryItemMock.Setup(i => i.CommitReservation(reservationId)).Returns(false);

            _repositoryMock.Setup(r => r.GetByStoreAndSkuAsync(It.IsAny<StoreId>(), It.IsAny<Sku>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(inventoryItemMock.Object);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }
    }
}