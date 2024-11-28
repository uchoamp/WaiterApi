using FluentValidation;
using Waiter.Application.Exceptions;
using Waiter.Application.Models.Orders;
using Waiter.Domain.Enums;
using Waiter.Domain.Models;
using Waiter.Domain.Repositories;
using Waiter.Domains.Services;

namespace Waiter.Application.UseCases.Orders
{
    public class CreateOrderUseCase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICalculateOrderCostService _calculateOrderCostService;
        private readonly AbstractValidator<OrderRequest> _validator;

        public CreateOrderUseCase(
            IOrderRepository orderRepository,
            ICalculateOrderCostService calculateOrderCostService,
            AbstractValidator<OrderRequest> validator
        )
        {
            _orderRepository = orderRepository;
            _calculateOrderCostService = calculateOrderCostService;
            _validator = validator;
        }

        public async Task<OrderResponse> Create(OrderRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                throw new ApplicationValidationException(validationResult);
            }

            var items = request
                .Items.Select(x => new OrderItem { ItemId = x.ItemId, Quantity = x.Quantity, })
                .ToArray();

            var order = new Order
            {
                Status = OrderStatus.Pending,
                CustomerId = request.CustomerId,
                Items = items,
                TotalPriceCents = await _calculateOrderCostService.Calculate(items)
            };

            await _orderRepository.CreateAsync(order);
            await _orderRepository.SaveChangesAsync();
            await _orderRepository.RefreshAsync(order);

            return new OrderResponse(
                order.Id,
                order.CustomerId,
                order.CreatedAt,
                order.Customer.FullName,
                order.TotalPriceCents,
                order.Status,
                order.Status.ToString(),
                order
                    .Items.Select(x => new OrderItemResponse(x.ItemId, x.Item.Name, x.Quantity))
                    .ToArray()
            );
        }
    }
}
