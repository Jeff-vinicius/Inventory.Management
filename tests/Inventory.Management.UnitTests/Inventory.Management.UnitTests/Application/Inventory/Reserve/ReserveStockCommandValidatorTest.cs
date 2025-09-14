using FluentAssertions;
using Inventory.Management.Application.Inventory.Reserve;

namespace Inventory.Management.UnitTests.Application.Inventory.Reserve
{
    public class ReserveStockCommandValidatorTest
    {
        private ReserveStockCommandValidator CreateValidator()
            => new ReserveStockCommandValidator();

        [Fact]
        public void Should_Validate_Valid_Command()
        {
            var command = new ReserveStockCommand(
                1,
                "SKU-123",
                10,
                "ORDER-001"
            );
            var validator = CreateValidator();

            var result = validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Should_Fail_When_StoreId_Is_Zero()
        {
            var command = new ReserveStockCommand(
                0,
                "SKU-123",
                10,
                "ORDER-001"
            );
            var validator = CreateValidator();

            var result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "StoreId" && e.ErrorMessage == "Store ID deve ser maior que zero");
        }

        [Theory]
        [InlineData("")]
        public void Should_Fail_When_Sku_Is_Empty(string sku)
        {
            var command = new ReserveStockCommand(
                1,
                sku,
                10,
                "ORDER-001"
            );
            var validator = CreateValidator();

            var result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Sku" && e.ErrorMessage == "SKU é obrigatório");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Fail_When_Quantity_Is_Invalid(int quantity)
        {
            var command = new ReserveStockCommand(
                1,
                "SKU-123",
                quantity,
                "ORDER-001"
            );
            var validator = CreateValidator();

            var result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Quantity" && e.ErrorMessage == "Quantidade deve ser maior que zero");
        }

        [Theory]
        [InlineData("")]
        public void Should_Fail_When_OrderId_Is_Empty(string orderId)
        {
            var command = new ReserveStockCommand(
                1,
                "SKU-123",
                10,
                orderId
            );
            var validator = CreateValidator();

            var result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "OrderId" && e.ErrorMessage == "Order ID é obrigatório");
        }
    }
}