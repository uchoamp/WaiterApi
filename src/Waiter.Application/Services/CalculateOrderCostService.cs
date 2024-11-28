using Waiter.Domain.Models;
using Waiter.Domain.Repositories;
using Waiter.Domains.Services;

namespace Waiter.Application.Services
{
    public class CalculateOrderCostService : ICalculateOrderCostService
    {
        private readonly IMenuItemRepository _menuItemRepository;

        public CalculateOrderCostService(IMenuItemRepository menuItemRepository)
        {
            _menuItemRepository = menuItemRepository;
        }

        public async Task<int> Calculate(OrderItem[] items)
        {
            var ids = items.Select(x => x.ItemId).ToArray();

            var itemsPrice = (await _menuItemRepository.GetEntitiesWithIds(ids)).ToDictionary(
                e => e.Id,
                e => e.PriceCents
            );

            return items.Select(x => x.Quantity * itemsPrice[x.ItemId]).Sum();
        }
    }
}
