using FluentAssertions;
using Inventory.Management.Domain.ValueObjects;

namespace Inventory.Management.UnitTests.Domain.ValueObjects
{
    public class BatchIdTest
    {
        [Fact]
        public void Constructor_WithValidValue_ShouldSetValue()
        {
            var batchId = new BatchId("BATCH-123");
            batchId.Value.Should().Be("BATCH-123");
        }

        [Fact]
        public void ImplicitConversion_ToString_ShouldReturnValue()
        {
            BatchId batchId = new BatchId("BATCH-456");
            string value = batchId;
            value.Should().Be("BATCH-456");
        }

        [Fact]
        public void ImplicitConversion_FromString_ShouldReturnBatchId()
        {
            BatchId batchId = "BATCH-789";
            batchId.Value.Should().Be("BATCH-789");
        }

        [Fact]
        public void ToString_ShouldReturnValue()
        {
            var batchId = new BatchId("BATCH-321");
            batchId.ToString().Should().Be("BATCH-321");
        }
    }
}