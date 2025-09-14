using FluentAssertions;
using Inventory.Management.Application.Inventory.GetAvailability;

namespace Inventory.Management.UnitTests.Application.Inventory.GetAvailability
{
    public class GetSkuAvailabilityQueryValidatorTest
    {
        private GetSkuAvailabilityQueryValidator CreateValidator()
            => new GetSkuAvailabilityQueryValidator();

        [Fact]
        public void Should_Validate_Valid_Query()
        {
            var query = new GetSkuAvailabilityQuery(1, "SKU-123");
            var validator = CreateValidator();

            var result = validator.Validate(query);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Should_Fail_When_StoreId_Is_Zero()
        {
            var query = new GetSkuAvailabilityQuery(0, "SKU-123");
            var validator = CreateValidator();

            var result = validator.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "StoreId" && e.ErrorMessage == "Store ID must be greater than zero!");
        }

        [Theory]
        [InlineData("")]
        public void Should_Fail_When_Sku_Is_Empty(string sku)
        {
            var query = new GetSkuAvailabilityQuery(1, sku);
            var validator = CreateValidator();

            var result = validator.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Sku" && e.ErrorMessage == "SKU is required!");
        }

        [Fact]
        public void Should_Fail_When_Sku_Exceeds_MaxLength()
        {
            var query = new GetSkuAvailabilityQuery(1, new string('A', 21));
            var validator = CreateValidator();

            var result = validator.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Sku" && e.ErrorMessage == "SKU must have a maximum of 20 characters!");
        }
    }
}
