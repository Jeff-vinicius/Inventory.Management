using FluentAssertions;
using Inventory.Management.Domain.ValueObjects;
using Inventory.Management.Domain.Common;

namespace Inventory.Management.UnitTests.Domain.ValueObjects
{
    public class SkuTest
    {
        [Fact]
        public void Constructor_WithValidValue_ShouldSetValue()
        {
            var sku = new Sku("SKU-123");
            sku.Value.Should().Be("SKU-123");
        }

        [Fact]
        public void ImplicitConversion_ToString_ShouldReturnValue()
        {
            Sku sku = new Sku("SKU-456");
            string value = sku;
            value.Should().Be("SKU-456");
        }

        [Fact]
        public void ImplicitConversion_FromString_ShouldReturnSku()
        {
            Sku sku = "SKU-789";
            sku.Value.Should().Be("SKU-789");
        }

        [Fact]
        public void ToString_ShouldReturnValue()
        {
            var sku = new Sku("SKU-321");
            sku.ToString().Should().Be("SKU-321");
        }
    }
}