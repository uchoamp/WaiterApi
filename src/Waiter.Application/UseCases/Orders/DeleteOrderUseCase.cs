using Waiter.Application.Models.Common;
using Waiter.Domain.Repositories;

namespace Waiter.Application.UseCases.Orders
{
    public class DeleteOrderUseCase
    {
        private readonly IOrderRepository _orderRepository;

        public DeleteOrderUseCase(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<MessageResponse> Delete(Guid id)
        {
            if (await _orderRepository.ExistsEntity(id))
            {
                var order = await _orderRepository.GetByIdAsync(id);

                _orderRepository.Delete(order);

                await _orderRepository.SaveChangesAsync();
            }

            return new MessageResponse("Order has been deleted.");
        }
    }
}
