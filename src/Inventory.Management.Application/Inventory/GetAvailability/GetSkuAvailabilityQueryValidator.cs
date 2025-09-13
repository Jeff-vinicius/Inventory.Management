using FluentValidation;

namespace Inventory.Management.Application.Inventory.GetAvailability
{
    public class GetSkuAvailabilityQueryValidator : AbstractValidator<GetSkuAvailabilityQuery>
    {
        public GetSkuAvailabilityQueryValidator()
        {
            RuleFor(x => x.StoreId)
                .GreaterThan(0)
                .WithMessage("Store ID must be greater than zero!");

            RuleFor(x => x.Sku)
                .NotEmpty()
                .WithMessage("SKU is required!")
                .MaximumLength(20)
                .WithMessage("SKU must have a maximum of 20 characters!");
        }
    }
}