using Waiter.Application.Models.Customers;
using Waiter.Application.Models.Orders;

namespace Waiter.Application.Models.CustomerOrders
{
    public record CustomerOrderRequest(
        string FirstName,
        string LastName,
        string PhoneNumber,
        OrderItemRequest[] Items
    ) : CustomerRequest(FirstName, LastName, PhoneNumber) { }
}
