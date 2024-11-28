namespace Waiter.Application.Models.Users
{
    public record UserResponse(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        string PhoneNumber,
        string?[] Roles
    ) { }
}
