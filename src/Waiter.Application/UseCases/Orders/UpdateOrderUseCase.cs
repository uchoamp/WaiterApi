using FluentValidation;
using Waiter.Application.Exceptions;
using Waiter.Application.Models.Orders;
using Waiter.Application.Validators.Orders;
using Waiter.Domain.Models;
using Waiter.Domain.Repositories;
using Waiter.Domains.Services;

namespace Waiter.Application.UseCases.Orders
{
    public class UpdateOrderUseCase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICalculateOrderCostService _calculateOrderCostService;
        private readonly AbstractValidator<OrderRequest> _validator;

        public UpdateOrderUseCase(
            IOrderRepository orderRepository,
            ICalculateOrderCostService calculateOrderCostService,
            AbstractValidator<OrderRequest> validator
        )
        {
            _orderRepository = orderRepository;
            _calculateOrderCostService = calculateOrderCostService;
            _validator = validator;
        }

        public async Task<OrderResponse> Update(Guid id, OrderRequest orderRequest)
        {
            if (!await _orderRepository.ExistsEntity(id))
                throw new ResourceNotFoundException("Order");

            var validationResult = await _validator.ValidateAsync(orderRequest);

            if (!validationResult.IsValid)
                throw new ApplicationValidationException(validationResult);

            var order = await _orderRepository.GetByIdAsync(id);

            order.CustomerId = orderRequest.CustomerId;
            UpdateOrderItems(order.Items, orderRequest.Items);
            order.TotalPriceCents = await _calculateOrderCostService.Calculate(
                order.Items.ToArray()
            );

            _orderRepository.Update(order);
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

        private static void UpdateOrderItems(
            ICollection<OrderItem> currentItems,
            ICollection<OrderItemRequest> requestItems
        )
        {
            var currentItemsMap = currentItems.ToDictionary(x => x.ItemId, x => x);
            var modifiedItemsMap = requestItems.ToDictionary(x => x.ItemId, x => x);

            var allIds = currentItems
                .Select(x => x.ItemId)
                .Concat(requestItems.Select(x => x.ItemId))
                .ToHashSet();

            foreach (var itemId in allIds)
            {
                currentItemsMap.TryGetValue(itemId, out var currentItem);
                modifiedItemsMap.TryGetValue(itemId, out var modifiedItem);

                if (currentItem != null && modifiedItem != null)
                {
                    currentItem.Quantity = modifiedItem.Quantity;
                }
                else if (currentItem != null)
                {
                    currentItems.Remove(currentItem);
                }
                else if (modifiedItem != null)
                {
                    currentItems.Add(
                        new OrderItem
                        {
                            ItemId = modifiedItem.ItemId,
                            Quantity = modifiedItem.Quantity
                        }
                    );
                }
            }
        }
    }
}
