using Waiter.Application.Exceptions;
using Waiter.Application.Models.Common;
using Waiter.Domain.Enums;
using Waiter.Domain.Repositories;

namespace Waiter.Application.UseCases.Orders
{
    public class UpdateOrderStatusUseCase
    {
        private readonly IOrderRepository _orderRepository;

        public UpdateOrderStatusUseCase(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<MessageResponse> UpdateStatus(Guid id, OrderStatus newStatus)
        {
            if (!await _orderRepository.ExistsEntity(id))
                throw new ResourceNotFoundException("Order");

            var order = await _orderRepository.GetByIdAsync(id);
            var currentOrderStatus = order.Status;

            order.Status = newStatus;

            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync();

            return new MessageResponse(
                $"Order status changed from {currentOrderStatus.ToString()} to {order.Status.ToString()}."
            );
        }
    }
}
