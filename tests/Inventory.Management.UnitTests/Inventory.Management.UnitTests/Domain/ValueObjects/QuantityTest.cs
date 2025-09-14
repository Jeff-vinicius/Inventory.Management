using FluentAssertions;
using Inventory.Management.Domain.ValueObjects;
using Inventory.Management.Domain.Common;

namespace Inventory.Management.UnitTests.Domain.ValueObjects
{
    public class QuantityTest
    {
        [Fact]
        public void Constructor_WithValidValue_ShouldSetValue()
        {
            var quantity = new Quantity(5);
            quantity.Value.Should().Be(5);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Constructor_WithInvalidValue_ShouldThrowDomainException(int value)
        {
            var act = () => new Quantity(value);
            act.Should().Throw<DomainException>()
                .WithMessage("Quantity must be greater than zero!");
        }

        [Fact]
        public void FromInt_ShouldReturnQuantity()
        {
            var quantity = Quantity.FromInt(10);
            quantity.Value.Should().Be(10);
        }

        [Fact]
        public void ImplicitConversion_ToInt_ShouldReturnValue()
        {
            Quantity q = new Quantity(7);
            int value = q;
            value.Should().Be(7);
        }

        [Fact]
        public void ImplicitConversion_FromInt_ShouldReturnQuantity()
        {
            Quantity q = 8;
            q.Value.Should().Be(8);
        }

        [Fact]
        public void ToString_ShouldReturnValueAsString()
        {
            var quantity = new Quantity(3);
            quantity.ToString().Should().Be("3");
        }
    }
}