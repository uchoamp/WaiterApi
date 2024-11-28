using Waiter.Application.Exceptions;
using Waiter.Application.Models.CustomerOrders;
using Waiter.Application.Models.Orders;
using Waiter.Application.UseCases.Customers;
using Waiter.Application.UseCases.Orders;
using Waiter.Application.Validators.CustomerOrders;
using Waiter.Domain.Repositories;
using Waiter.Domains.Services;

namespace Waiter.Application.UseCases.CustomerOrders
{
    public class CreateCustomerOrderUseCase
    {
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly CreateOrderUseCase _createOrderUseCase;
        private readonly CreateCustomerUseCase _createCustomerUseCase;
        private readonly UpdateCustomerUseCase _updateCustomerUseCase;

        public CreateCustomerOrderUseCase(
            ICalculateOrderCostService calculateOrderCostService,
            IMenuItemRepository menuItemRepository,
            ICustomerRepository customerRepository,
            CreateOrderUseCase createOrderUseCase,
            CreateCustomerUseCase createCustomerUseCase,
            UpdateCustomerUseCase updateCustomerUseCase
        )
        {
            _menuItemRepository = menuItemRepository;
            _customerRepository = customerRepository;
            _createCustomerUseCase = createCustomerUseCase;
            _createOrderUseCase = createOrderUseCase;
            _updateCustomerUseCase = updateCustomerUseCase;
        }

        public async Task<OrderResponse> Create(CustomerOrderRequest request)
        {
            var customerId = await _customerRepository.FindIdWithPhoneNumber(request.PhoneNumber);

            var validator = new CustomerOrderRequestValidator(
                customerId,
                _menuItemRepository,
                _customerRepository
            );
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                throw new ApplicationValidationException(validationResult);
            }

            if (customerId == Guid.Empty)
            {
                var newCustomer = await _createCustomerUseCase.Create(request);
                customerId = newCustomer.Id;
            }
            else
            {
                await _updateCustomerUseCase.Update(customerId, request);
            }

            return await _createOrderUseCase.Create(new OrderRequest(customerId, request.Items));
        }
    }
}
