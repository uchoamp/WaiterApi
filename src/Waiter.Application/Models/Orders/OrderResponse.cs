using Waiter.Domain.Enums;

namespace Waiter.Application.Models.Orders
{
    public record OrderResponse(
        Guid Id,
        Guid CustomerId,
        DateTime CreatedAt,
        string CustomerName,
        int TotalPriceCents,
        OrderStatus Status,
        string StatusDescription,
        OrderItemResponse[] Items
    ) { }
}
