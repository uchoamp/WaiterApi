using System.ComponentModel.DataAnnotations.Schema;

namespace Waiter.Domain.Models;

[Table("OrderItems")]
public class OrderItem
{
    public Guid OrderId { get; set; }
    public Guid ItemId { get; set; }
    public int Quantity { get; set; }

    public Order Order { get; set; }
    public MenuItem Item { get; set; }
}
