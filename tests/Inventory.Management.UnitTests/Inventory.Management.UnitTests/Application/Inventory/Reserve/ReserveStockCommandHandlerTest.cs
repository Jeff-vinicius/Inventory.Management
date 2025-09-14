using FluentAssertions;
using Inventory.Management.Application.Inventory.Reserve;
using Inventory.Management.Domain.Aggregates;
using Inventory.Management.Domain.Entities;
using Inventory.Management.Domain.Errors;
using Inventory.Management.Domain.Interfaces;
using Inventory.Management.Domain.ValueObjects;
using Moq;

namespace Inventory.Management.UnitTests.Application.Inventory.Reserve
{
    public class ReserveStockCommandHandlerTest
    {
        private readonly Mock<IInventoryRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly ReserveStockCommandHandler _handler;

        public ReserveStockCommandHandlerTest()
        {
            _repositoryMock = new Mock<IInventoryRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new ReserveStockCommandHandler(_repositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenInventoryItemNotFound()
        {
            // Arrange
            var command = new ReserveStockCommand(1, "sku-123", 2, "order-1");
            _repositoryMock
                .Setup(r => r.GetByStoreAndSkuAsync(It.IsAny<StoreId>(), It.IsAny<Sku>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((InventoryItem?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeEquivalentTo(InventoryErrors.NotFound(command.StoreId, command.Sku));

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<InventoryItem>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenReservationSucceeds()
        {
            // Arrange
            var command = new ReserveStockCommand(1, "sku-123", 2, "order-1");
            var inventoryItemMock = new Mock<InventoryItem>(new StoreId(command.StoreId), new Sku(command.Sku), 0);

            var orderId = new OrderId(command.OrderId);
            var quantity = new Quantity(command.Quantity);

            var fakeReservation = new Reservation(orderId, quantity);
            inventoryItemMock.Setup(i => i.Reserve(It.IsAny<OrderId>(), It.IsAny<Quantity>()))
                             .Returns(fakeReservation);

            _repositoryMock
                .Setup(r => r.GetByStoreAndSkuAsync(It.IsAny<StoreId>(), It.IsAny<Sku>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(inventoryItemMock.Object);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(new ReservationResponse(
                fakeReservation.ReservationId,
                inventoryItemMock.Object.Version,
                "reserved"
            ));

            inventoryItemMock.Verify(i => i.Reserve(It.Is<OrderId>(o => o.Value == command.OrderId),
                                                    It.Is<Quantity>(q => q.Value == command.Quantity)),
                                     Times.Once);

            _repositoryMock.Verify(r => r.UpdateAsync(inventoryItemMock.Object, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

