using Microsoft.AspNetCore.Identity;

namespace Waiter.Domain.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; }
}
