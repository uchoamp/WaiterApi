using Waiter.Application.Models.MenuItems;
using Waiter.Domain.Models;
using Waiter.Domain.Repositories;

namespace Waiter.Application.UseCases.MenuItems
{
    public class GetMenuItemsPaginatedUseCase
    {
        private readonly IMenuItemRepository _menuItemRepository;

        public GetMenuItemsPaginatedUseCase(IMenuItemRepository menuItemRepository)
        {
            _menuItemRepository = menuItemRepository;
        }

        public async Task<MenuItemsPaginatedResponse> Get(int currentPage, int pageSize)
        {
            var result = await _menuItemRepository.PaginateAsync(
                new PaginationLimits(currentPage, pageSize)
            );

            return new MenuItemsPaginatedResponse(
                currentPage,
                pageSize,
                result.LastPage,
                result
                    .Result.Select(x => new MenuItemResponse(x.Id, x.Name, x.PriceCents))
                    .ToArray()
            );
        }
    }
}
