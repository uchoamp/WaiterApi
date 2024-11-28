using Waiter.Application.Models.Common;
using Waiter.Domain.Repositories;

namespace Waiter.Application.UseCases.MenuItems
{
    public class DeleteMenuItemUseCase
    {
        private readonly IMenuItemRepository _menuItemRepository;

        public DeleteMenuItemUseCase(IMenuItemRepository menuItemRepository)
        {
            _menuItemRepository = menuItemRepository;
        }

        public async Task<MessageResponse> Delete(Guid id)
        {
            if (await _menuItemRepository.ExistsEntity(id))
            {
                var menuItem = await _menuItemRepository.GetByIdAsync(id);

                _menuItemRepository.Delete(menuItem);

                await _menuItemRepository.SaveChangesAsync();
            }

            return new MessageResponse("Menu item has been deleted.");
        }
    }
}
