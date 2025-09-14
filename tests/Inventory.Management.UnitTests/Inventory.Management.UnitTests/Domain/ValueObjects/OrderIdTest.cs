using FluentAssertions;
using Inventory.Management.Domain.ValueObjects;
using Inventory.Management.Domain.Common;

namespace Inventory.Management.UnitTests.Domain.ValueObjects
{
    public class OrderIdTest
    {
        [Fact]
        public void Constructor_WithValidValue_ShouldSetValue()
        {
            var orderId = new OrderId("ORDER-123");
            orderId.Value.Should().Be("ORDER-123");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Constructor_WithInvalidValue_ShouldThrowDomainException(string value)
        {
            var act = () => new OrderId(value!);
            act.Should().Throw<DomainException>()
                .WithMessage("OrderId cannot be empty!");
        }

        [Fact]
        public void ImplicitConversion_ToString_ShouldReturnValue()
        {
            OrderId orderId = new OrderId("ORDER-456");
            string value = orderId;
            value.Should().Be("ORDER-456");
        }

        [Fact]
        public void ImplicitConversion_FromString_ShouldReturnOrderId()
        {
            OrderId orderId = "ORDER-789";
            orderId.Value.Should().Be("ORDER-789");
        }

        [Fact]
        public void ToString_ShouldReturnValue()
        {
            var orderId = new OrderId("ORDER-321");
            orderId.ToString().Should().Be("ORDER-321");
        }
    }
}