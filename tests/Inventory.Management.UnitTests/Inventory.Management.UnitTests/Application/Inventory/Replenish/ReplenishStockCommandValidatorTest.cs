using FluentAssertions;
using Inventory.Management.Application.Inventory.Replenish;

namespace Inventory.Management.UnitTests.Application.Inventory.Replenish
{
    public class ReplenishStockCommandValidatorTest
    {
        private ReplenishStockCommandValidator CreateValidator()
            => new ReplenishStockCommandValidator();

        [Fact]
        public void Should_Validate_Valid_Command()
        {
            var command = new ReplenishStockCommand(
                1,
                "SKU-123",
                10,
                "BATCH-001"
            );
            var validator = CreateValidator();

            var result = validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Should_Fail_When_StoreId_Is_Zero()
        {
            var command = new ReplenishStockCommand(
                0,
                "SKU-123",
                10,
                "BATCH-001"
            );
            var validator = CreateValidator();

            var result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "StoreId" && e.ErrorMessage == "Store ID must be greater than zero!");
        }

        [Theory]
        [InlineData("")]
        public void Should_Fail_When_Sku_Is_Empty(string sku)
        {
            var command = new ReplenishStockCommand(
                1,
                sku,
                10,
                "BATCH-001"
            );
            var validator = CreateValidator();

            var result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Sku" && e.ErrorMessage == "SKU is required!");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Fail_When_Quantity_Is_Invalid(int quantity)
        {
            var command = new ReplenishStockCommand(
                1,
                "SKU-123",
                quantity,
                "BATCH-001"
            );
            var validator = CreateValidator();

            var result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Quantity" && e.ErrorMessage == "Quantity must be greater than zero!");
        }

        [Theory]
        [InlineData("")]
        public void Should_Fail_When_BatchId_Is_Empty(string batchId)
        {
            var command = new ReplenishStockCommand(
                1,
                "SKU-123",
                10,
                batchId
            );
            var validator = CreateValidator();

            var result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "BatchId" && e.ErrorMessage == "Batch ID is required!");
        }
    }
}