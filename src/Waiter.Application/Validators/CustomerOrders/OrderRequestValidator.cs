using FluentValidation;
using Waiter.Application.Models.CustomerOrders;
using Waiter.Application.Validators.Customers;
using Waiter.Application.Validators.Orders;
using Waiter.Domain.Repositories;

namespace Waiter.Application.Validators.CustomerOrders
{
    public class CustomerOrderRequestValidator : AbstractValidator<CustomerOrderRequest>
    {
        public CustomerOrderRequestValidator(
            Guid id,
            IMenuItemRepository menuItemRepository,
            ICustomerRepository customerRepository
        )
        {
            Include(new CustomerRequestValidator(id, customerRepository));

            RuleFor(x => x.Items)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithErrorCode("ItemsRequired")
                .WithMessage("Items are required.")
                .SetValidator(new OrderItemsRequestValidator(menuItemRepository));
        }
    }
}
