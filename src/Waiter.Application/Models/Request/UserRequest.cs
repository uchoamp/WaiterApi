namespace Waiter.Application.Models.Request
{
    public record UserRequest(
        Guid? Id,
        string FirstName,
        string LastName,
        string Email,
        string Password,
        string[] Roles
    ) { }
}
