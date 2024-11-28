namespace Waiter.Application.Models.Orders
{
    public record OrderRequest(Guid CustomerId, OrderItemRequest[] Items) { }
}
