using Waiter.Application.Exceptions;
using Waiter.Application.Models.MenuItems;
using Waiter.Domain.Repositories;

namespace Waiter.Application.UseCases.MenuItems
{
    public class GetMenuItemUseCase
    {
        private readonly IMenuItemRepository _menuItemRepository;

        public GetMenuItemUseCase(IMenuItemRepository menuItemRepository)
        {
            _menuItemRepository = menuItemRepository;
        }

        public async Task<MenuItemResponse> Get(Guid id)
        {
            if (!await _menuItemRepository.ExistsEntity(id))
                throw new ResourceNotFoundException("MenuItem");

            var menuItem = await _menuItemRepository.GetByIdAsync(id);

            return new MenuItemResponse(menuItem.Id, menuItem.Name, menuItem.PriceCents);
        }
    }
}
