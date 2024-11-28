using FluentValidation;
using Waiter.Application.Models.MenuItems;

namespace Waiter.Application.Validators.MenuItems
{
    public class MenuItemRequestValidator : AbstractValidator<MenuItemRequest>
    {
        public MenuItemRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .WithErrorCode("NameRequired")
                .MinimumLength(2)
                .WithMessage("Name must be at least 2 characters.")
                .WithErrorCode("NameInvalid");

            RuleFor(x => x.PriceCents)
                .GreaterThan(0)
                .WithMessage("Price must be greater then 0.")
                .WithErrorCode("PriceCentsZero");
        }
    }
}
