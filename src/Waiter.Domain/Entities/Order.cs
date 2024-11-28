using System.ComponentModel.DataAnnotations.Schema;
using Waiter.Domain.Enums;

namespace Waiter.Domain.Models;

[Table("Orders")]
public class Order : BaseEntity
{
    public OrderStatus Status { get; set; }
    public int TotalPriceCents { get; set; }
    public Guid CustomerId { get; set; }
    public Guid UpdatedBy { get; set; }

    public Customer Customer { get; set; }
    public ICollection<OrderItem> Items { get; set; }
}
