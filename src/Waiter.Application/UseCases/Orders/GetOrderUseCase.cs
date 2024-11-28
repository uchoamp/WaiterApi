using Waiter.Application.Exceptions;
using Waiter.Application.Models.Orders;
using Waiter.Domain.Repositories;

namespace Waiter.Application.UseCases.Orders
{
    public class GetOrderUseCase
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderUseCase(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<OrderResponse> Get(Guid id)
        {
            if (!await _orderRepository.ExistsEntity(id))
                throw new ResourceNotFoundException("Order");

            var order = await _orderRepository.GetByIdAsync(id);

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
