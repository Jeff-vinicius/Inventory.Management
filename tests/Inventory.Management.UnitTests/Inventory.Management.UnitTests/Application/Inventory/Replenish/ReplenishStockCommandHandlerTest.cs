using FluentAssertions;
using Inventory.Management.Application.Inventory.Replenish;
using Inventory.Management.Domain.Aggregates;
using Inventory.Management.Domain.Interfaces;
using Inventory.Management.Domain.ValueObjects;
using Moq;

namespace Inventory.Management.UnitTests.Application.Inventory.Replenish
{
    public class ReplenishStockCommandHandlerTest
    {
        private readonly Mock<IInventoryRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly ReplenishStockCommandHandler _handler;

        public ReplenishStockCommandHandlerTest()
        {
            _repositoryMock = new Mock<IInventoryRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new ReplenishStockCommandHandler(_repositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldAddNewInventoryItem_WhenItemNotFound()
        {
            // Arrange
            var command = new ReplenishStockCommand(1, "sku-123", 5, "batch-1");

            _repositoryMock
                .Setup(r => r.GetByStoreAndSkuAsync(It.IsAny<StoreId>(), It.IsAny<Sku>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((InventoryItem?)null);

            InventoryItem? capturedItem = null;
            _repositoryMock
                .Setup(r => r.AddAsync(It.IsAny<InventoryItem>(), It.IsAny<CancellationToken>()))
                .Callback<InventoryItem, CancellationToken>((item, _) => capturedItem = item)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeTrue();

            capturedItem.Should().NotBeNull();
            capturedItem!.StoreId.Value.Should().Be(command.StoreId);
            capturedItem.Sku.Value.Should().Be(command.Sku);

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<InventoryItem>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReplenishExistingItem_WhenItemFound()
        {
            // Arrange
            var command = new ReplenishStockCommand(1, "sku-123", 5, "batch-1");
            var existingItemMock = new Mock<InventoryItem>(new StoreId(command.StoreId), new Sku(command.Sku));

            existingItemMock.Setup(i => i.Replenish(command.Quantity, command.BatchId));

            _repositoryMock
                .Setup(r => r.GetByStoreAndSkuAsync(It.IsAny<StoreId>(), It.IsAny<Sku>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingItemMock.Object);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeTrue();

            existingItemMock.Verify(i => i.Replenish(command.Quantity, command.BatchId), Times.Once);

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<InventoryItem>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
