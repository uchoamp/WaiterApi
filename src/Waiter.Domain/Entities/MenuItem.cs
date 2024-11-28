using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Waiter.Domain.Models;

[Table("MenuItems")]
public class MenuItem : BaseEntity
{
    [MaxLength(256)]
    public required string Name { get; set; }

    public required int PriceCents { get; set; }
}
