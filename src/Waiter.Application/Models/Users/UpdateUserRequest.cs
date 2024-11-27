namespace Waiter.Application.Models.Users
{
    public record UpdateUserRequest(
        Guid Id,
        string FirstName,
        string LastName,
        string PhoneNumber,
        string Email
    ) { }
}
