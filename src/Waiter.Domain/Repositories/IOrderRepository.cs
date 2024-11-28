using Waiter.Domain.Models;

namespace Waiter.Domain.Repositories
{
    public interface IOrderRepository : IRepository<Order> { }

    public interface IOrderRepository<in TFilter> : IRepository<Order, TFilter> { }
}
