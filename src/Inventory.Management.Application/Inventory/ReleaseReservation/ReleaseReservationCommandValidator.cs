using FluentValidation;
using Inventory.Management.Application.Inventory.Release;

namespace Inventory.Management.Application.Inventory.ReleaseReservation
{
    public class ReleaseReservationCommandValidator : AbstractValidator<ReleaseReservationCommand>
    {
        public ReleaseReservationCommandValidator()
        {
            RuleFor(x => x.StoreId)
                .GreaterThan(0)
                .WithMessage("Store ID must be greater than zero!");

            RuleFor(x => x.Sku)
                .NotEmpty()
                .WithMessage("SKU is required!");

            RuleFor(x => x.ReservationId)
                .NotEmpty()
                .WithMessage("Reservation ID is required!");
        }
    }
}
