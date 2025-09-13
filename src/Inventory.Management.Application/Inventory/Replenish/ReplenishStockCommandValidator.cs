using FluentValidation;

namespace Inventory.Management.Application.Inventory.Replenish
{
    public class ReplenishStockCommandValidator : AbstractValidator<ReplenishStockCommand>
    {
        public ReplenishStockCommandValidator()
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

            RuleFor(x => x.BatchId)
                .NotEmpty()
                .WithMessage("Batch ID is required!");
        }
    }
}
