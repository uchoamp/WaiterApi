namespace Waiter.Application.Models.Request
{
    public record NewUserRequest(
        string FirstName,
        string LastName,
        string Email,
        string Password,
        string[] Roles
    ) { }
}
