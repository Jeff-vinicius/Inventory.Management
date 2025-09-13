using FluentValidation;

namespace Inventory.Management.Application.Inventory.Commit
{
    public class CommitReservationCommandValidator : AbstractValidator<CommitReservationCommand>
    {
        public CommitReservationCommandValidator()
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

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than zero!");
        }
    }
}