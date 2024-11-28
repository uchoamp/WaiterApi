using Waiter.Application.Exceptions;
using Waiter.Application.Models.MenuItems;
using Waiter.Application.Validators.MenuItems;
using Waiter.Domain.Models;
using Waiter.Domain.Repositories;

namespace Waiter.Application.UseCases.MenuItems
{
    public class CreateMenuItemUseCase
    {
        private readonly IMenuItemRepository _menuItemRepository;

        public CreateMenuItemUseCase(IMenuItemRepository menuItemRepository)
        {
            _menuItemRepository = menuItemRepository;
        }

        public async Task<MenuItemResponse> Create(MenuItemRequest request)
        {
            var validator = new MenuItemRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                throw new ApplicationValidationException(validationResult);
            }

            var entity = new MenuItem { Name = request.Name, PriceCents = request.PriceCents };

            await _menuItemRepository.CreateAsync(entity);
            await _menuItemRepository.SaveChangesAsync();

            return new MenuItemResponse(entity.Id, entity.Name, entity.PriceCents);
        }
    }
}
