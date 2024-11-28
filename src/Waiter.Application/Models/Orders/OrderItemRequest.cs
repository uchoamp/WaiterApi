namespace Waiter.Application.Models.Orders
{
    public record OrderItemRequest(Guid ItemId, int Quantity) { }
}
