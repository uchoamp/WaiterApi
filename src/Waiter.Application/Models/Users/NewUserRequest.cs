namespace Waiter.Application.Models.Users;

public record NewUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Password,
    string[] Roles
) { }
