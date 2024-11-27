namespace Waiter.Application.Models.Request
{
    public record UpdateUserRequest(Guid Id, string FirstName, string LastName, string Email) { }
}
