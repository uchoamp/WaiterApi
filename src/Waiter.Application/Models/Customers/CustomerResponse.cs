namespace Waiter.Application.Models.Customers
{
    public record CustomerResponse(
        Guid Id,
        string FirstName,
        string LastName,
        string PhoneNumber
    ) { }
}
