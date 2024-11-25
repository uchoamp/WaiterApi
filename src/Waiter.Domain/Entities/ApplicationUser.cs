using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Waiter.Domain.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    [MaxLength(256)]
    public required string FirstName { get; set; }

    [MaxLength(256)]
    public required string LastName { get; set; }
}
