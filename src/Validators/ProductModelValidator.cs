using FluentValidation;
using PriceNegotiationApp.Models.Dtos;

namespace PriceNegotiationApp.Validators;

public class ProductModelValidator : AbstractValidator<Product>
{
    public ProductModelValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(5).MaximumLength(255);
        RuleFor(x => x.Description).NotEmpty().MinimumLength(5).MaximumLength(1000);
        RuleFor(x => x.BasePrice).GreaterThan(0);
    }
}