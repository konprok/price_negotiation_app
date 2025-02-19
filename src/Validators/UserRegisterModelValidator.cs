using FluentValidation;
using PriceNegotiationApp.Models.Dtos;

namespace PriceNegotiationApp.Validators;

public sealed class UserRegisterModelValidator : AbstractValidator<UserRegisterDto>
{
    public UserRegisterModelValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().MinimumLength(3).MaximumLength(255);

        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty().MinimumLength(6).MaximumLength(255);
    }
}