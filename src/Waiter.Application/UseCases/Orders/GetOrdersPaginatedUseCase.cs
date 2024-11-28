using Waiter.Application.Models.Orders;
using Waiter.Domain.Models;
using Waiter.Domain.Repositories;

namespace Waiter.Application.UseCases.Orders
{
    public class GetOrdersPaginatedUseCase
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrdersPaginatedUseCase(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<OrdersPaginatedResponse> Get(int currentPage, int pageSize)
        {
            var result = await _orderRepository.PaginateAsync(
                new PaginationLimits(currentPage, pageSize)
            );

            return new OrdersPaginatedResponse(
                currentPage,
                pageSize,
                result.LastPage,
                result
                    .Result.Select(order => new OrderResponse(
                        order.Id,
                        order.CustomerId,
                        order.CreatedAt,
                        order.Customer.FullName,
                        order.TotalPriceCents,
                        order.Status,
                        order.Status.ToString(),
                        order
                            .Items.Select(x => new OrderItemResponse(
                                x.ItemId,
                                x.Item.Name,
                                x.Quantity
                            ))
                            .ToArray()
                    ))
                    .ToArray()
            );
        }
    }
}
