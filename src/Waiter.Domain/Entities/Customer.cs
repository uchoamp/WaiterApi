using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Waiter.Domain.Models;

[Table("Customers")]
public class Customer : BaseEntity
{
    [MaxLength(256)]
    public required string FirstName { get; set; }

    [MaxLength(256)]
    public required string LastName { get; set; }

    [MaxLength(15)]
    public required string PhoneNumber { get; set; }

    [NotMapped]
    public string FullName => FirstName + " " + LastName;

    public ICollection<Order> Orders { get; set; }
}
