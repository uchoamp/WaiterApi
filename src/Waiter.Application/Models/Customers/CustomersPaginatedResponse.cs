namespace Waiter.Application.Models.Customers
{
    public record CustomersPaginatedResponse(
        int CurrentPage,
        int PageSize,
        int LastPage,
        CustomerResponse[] Customers
    ) { }
}
