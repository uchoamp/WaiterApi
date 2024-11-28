namespace Waiter.Application.Models.Orders
{
    public record OrderItemResponse(Guid ItemId, string ItemName, int Quantity) { }
}
