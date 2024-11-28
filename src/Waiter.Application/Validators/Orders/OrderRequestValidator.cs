using FluentValidation;
using Waiter.Application.Models.Orders;
using Waiter.Domain.Repositories;

namespace Waiter.Application.Validators.Orders
{
    public class OrderRequestValidator : AbstractValidator<OrderRequest>
    {
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly ICustomerRepository _customerRepository;

        public OrderRequestValidator(
            IMenuItemRepository menuItemRepository,
            ICustomerRepository customerRepository
        )
        {
            _menuItemRepository = menuItemRepository;
            _customerRepository = customerRepository;

            RuleFor(x => x.CustomerId)
                .MustAsync(
                    async (customerId, cancelationToken) =>
                    {
                        return await _customerRepository.ExistsEntity(customerId);
                    }
                )
                .WithMessage(order => "Customer not found with Id " + order.CustomerId.ToString())
                .WithErrorCode("CustomerNotFound");

            RuleFor(x => x.Items)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithErrorCode("ItemsRequired")
                .WithMessage("Items are required.")
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
