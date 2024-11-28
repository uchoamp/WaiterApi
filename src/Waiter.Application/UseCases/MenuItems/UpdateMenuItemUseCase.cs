using Waiter.Application.Exceptions;
using Waiter.Application.Models.MenuItems;
using Waiter.Application.Validators.MenuItems;
using Waiter.Domain.Repositories;

namespace Waiter.Application.UseCases.MenuItems
{
    public class UpdateMenuItemUseCase
    {
        private readonly IMenuItemRepository _menuItemRepository;

        public UpdateMenuItemUseCase(IMenuItemRepository menuItemRepository)
        {
            _menuItemRepository = menuItemRepository;
        }

        public async Task<MenuItemResponse> Update(Guid id, MenuItemRequest menuItemRequest)
        {
            if (!await _menuItemRepository.ExistsEntity(id))
                throw new ResourceNotFoundException("MenuItem");

            var validator = new MenuItemRequestValidator();
            var validationResult = await validator.ValidateAsync(menuItemRequest);

            if (!validationResult.IsValid)
                throw new ApplicationValidationException(validationResult);

            var menuItem = await _menuItemRepository.GetByIdAsync(id);

            menuItem.Name = menuItemRequest.Name;
            menuItem.PriceCents = menuItemRequest.PriceCents;

            _menuItemRepository.Update(menuItem);
            await _menuItemRepository.SaveChangesAsync();

            return new MenuItemResponse(menuItem.Id, menuItem.Name, menuItem.PriceCents);
        }
    }
}
