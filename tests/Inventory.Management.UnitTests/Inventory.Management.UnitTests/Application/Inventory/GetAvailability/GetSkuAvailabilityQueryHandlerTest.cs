using FluentAssertions;
using Inventory.Management.Application.Inventory.GetAvailability;
using Inventory.Management.Domain.Aggregates;
using Inventory.Management.Domain.Errors;
using Inventory.Management.Domain.Interfaces;
using Inventory.Management.Domain.ValueObjects;
using Moq;

namespace Inventory.Management.UnitTests.Application.Inventory.GetAvailability
{
    public class GetSkuAvailabilityQueryHandlerTest
    {
        private readonly Mock<IInventoryRepository> _repositoryMock;
        private readonly GetSkuAvailabilityQueryHandler _handler;

        public GetSkuAvailabilityQueryHandlerTest()
        {
            _repositoryMock = new Mock<IInventoryRepository>();
            _handler = new GetSkuAvailabilityQueryHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenInventoryItemNotFound()
        {
            // Arrange
            var query = new GetSkuAvailabilityQuery(1, "SKU-123");
            _repositoryMock
                .Setup(r => r.GetByStoreAndSkuAsync(It.IsAny<StoreId>(), It.IsAny<Sku>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((InventoryItem?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeEquivalentTo(InventoryErrors.NotFound(query.StoreId, query.Sku));

            _repositoryMock.Verify(r =>
                r.GetByStoreAndSkuAsync(It.Is<StoreId>(id => id.Value == query.StoreId),
                                        It.Is<Sku>(sku => sku.Value == query.Sku),
                                        It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenInventoryItemExists()
        {
            // Arrange
            var query = new GetSkuAvailabilityQuery(1, "SKU-123");
            var inventoryItem = new InventoryItem(
                new StoreId(query.StoreId),
                new Sku(query.Sku)
            );

            _repositoryMock
                .Setup(r => r.GetByStoreAndSkuAsync(It.IsAny<StoreId>(), It.IsAny<Sku>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(inventoryItem);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(new SkuAvailabilityResponse(
                query.StoreId,
                query.Sku,
                inventoryItem.AvailableQuantity,
                inventoryItem.ReservedQuantity,
                inventoryItem.LastUpdatedAt
            ));

            _repositoryMock.Verify(r =>
                r.GetByStoreAndSkuAsync(It.Is<StoreId>(id => id.Value == query.StoreId),
                                        It.Is<Sku>(sku => sku.Value == query.Sku),
                                        It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
