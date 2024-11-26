namespace Waiter.Application.Models.Response
{
    public record UserResponse(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        string?[] Roles
    ) { }
}
