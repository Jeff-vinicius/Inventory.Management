using FluentValidation;

namespace Inventory.Management.Application.Inventory.Reserve
{
    public class ReserveStockCommandValidator : AbstractValidator<ReserveStockCommand>
    {
        public ReserveStockCommandValidator()
        {
            RuleFor(x => x.StoreId)
                .GreaterThan(0)
                .WithMessage("Store ID must be greater than zero!");

            RuleFor(x => x.Sku)
                .NotEmpty()
                .WithMessage("SKU is required!");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than zero!");

            RuleFor(x => x.OrderId)
                .NotEmpty()
                .WithMessage("Order ID is required!");
        }
    }
}