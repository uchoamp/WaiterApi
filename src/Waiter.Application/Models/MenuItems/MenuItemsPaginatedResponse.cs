namespace Waiter.Application.Models.MenuItems
{
    public record MenuItemsPaginatedResponse(
        int CurrentPage,
        int PageSize,
        int LastPage,
        MenuItemResponse[] MenuItems
    ) { }
}
