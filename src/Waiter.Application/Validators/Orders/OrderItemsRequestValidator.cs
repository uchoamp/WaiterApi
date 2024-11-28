using FluentValidation;
using Waiter.Application.Models.Orders;
using Waiter.Domain.Repositories;

namespace Waiter.Application.Validators.Orders
{
    public class OrderItemsRequestValidator : AbstractValidator<OrderItemRequest[]>
    {
        private readonly IMenuItemRepository _menuItemRepository;

        public OrderItemsRequestValidator(IMenuItemRepository menuItemRepository)
        {
            _menuItemRepository = menuItemRepository;

            RuleFor(x => x)
                .NotEmpty()
                .WithErrorCode("ItemsAtLeastOne")
                .WithMessage("At least one item must be informed.")
                .Must(items => items.Select(x => x.ItemId).Distinct().Count() == items.Length)
                .WithErrorCode("ItemsMustBeUnique")
                .WithMessage("Each item must be entered only once.")
                .ForEach(x =>
                    x.Must(x => x.Quantity > 0)
                        .WithErrorCode("ItemQuantityZero")
                        .WithMessage("Item quantity must be greater then zero.")
                        .MustAsync(
                            async (item, CancellationToken) =>
                            {
                                return await _menuItemRepository.ExistsEntity(item.ItemId);
                            }
                        )
                        .WithErrorCode("ItemNotFound")
                        .WithMessage((items, item) => "Item not found with Id " + item.ItemId)
                );
        }
    }
}
