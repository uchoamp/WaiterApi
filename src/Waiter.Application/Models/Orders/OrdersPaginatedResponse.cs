namespace Waiter.Application.Models.Orders
{
    public record OrdersPaginatedResponse(
        int CurrentPage,
        int PageSize,
        int LastPage,
        OrderResponse[] Orders
    ) { }
}
