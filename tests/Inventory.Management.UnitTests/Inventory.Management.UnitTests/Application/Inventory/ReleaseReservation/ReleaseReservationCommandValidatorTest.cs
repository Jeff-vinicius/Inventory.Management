using FluentAssertions;
using Inventory.Management.Application.Inventory.ReleaseReservation;

namespace Inventory.Management.UnitTests.Application.Inventory.ReleaseReservation
{
    public class ReleaseReservationCommandValidatorTest
    {
        private ReleaseReservationCommandValidator CreateValidator()
            => new ReleaseReservationCommandValidator();

        [Fact]
        public void Should_Validate_Valid_Command()
        {
            var command = new ReleaseReservationCommand(
                1,
                "SKU-123",
                "RES-001"
            );
            var validator = CreateValidator();

            var result = validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Should_Fail_When_StoreId_Is_Zero()
        {
            var command = new ReleaseReservationCommand(
                0,
                "SKU-123",
                "RES-001"
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
            var command = new ReleaseReservationCommand(
                1,
                sku,
                "RES-001"
            );
            var validator = CreateValidator();

            var result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Sku" && e.ErrorMessage == "SKU is required!");
        }

        [Theory]
        [InlineData("")]
        public void Should_Fail_When_ReservationId_Is_Empty(string reservationId)
        {
            var command = new ReleaseReservationCommand(
                1,
                "SKU-123",
                reservationId
            );
            var validator = CreateValidator();

            var result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ReservationId" && e.ErrorMessage == "Reservation ID is required!");
        }
    }
}