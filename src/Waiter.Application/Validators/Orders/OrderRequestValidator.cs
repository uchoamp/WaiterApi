using FluentValidation;
using Waiter.Application.Models.Orders;
using Waiter.Domain.Repositories;

namespace Waiter.Application.Validators.Orders
{
    public class OrderRequestValidator : AbstractValidator<OrderRequest>
    {
        private readonly ICustomerRepository _customerRepository;

        public OrderRequestValidator(
            IMenuItemRepository menuItemRepository,
            ICustomerRepository customerRepository
        )
        {
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
                .SetValidator(new OrderItemsRequestValidator(menuItemRepository));
        }
    }
}
