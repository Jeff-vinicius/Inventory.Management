using FluentValidation;

namespace Inventory.Management.Application.Inventory.Reserve
{
    public class ReserveStockCommandValidator : AbstractValidator<ReserveStockCommand>
    {
        public ReserveStockCommandValidator()
        {
            RuleFor(x => x.StoreId)
                .GreaterThan(0)
                .WithMessage("Store ID deve ser maior que zero");

            RuleFor(x => x.Sku)
                .NotEmpty()
                .WithMessage("SKU é obrigatório");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantidade deve ser maior que zero");

            RuleFor(x => x.OrderId)
                .NotEmpty()
                .WithMessage("Order ID é obrigatório");
        }
    }
}