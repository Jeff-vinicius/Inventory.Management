using FluentAssertions;
using Inventory.Management.Domain.ValueObjects;

namespace Inventory.Management.UnitTests.Domain.ValueObjects
{
    public class StoreIdTest
    {
        [Fact]
        public void Constructor_WithValidValue_ShouldSetValue()
        {
            var storeId = new StoreId(10);
            storeId.Value.Should().Be(10);
        }
    }
}