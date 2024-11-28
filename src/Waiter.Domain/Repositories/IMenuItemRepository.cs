using Waiter.Domain.Models;

namespace Waiter.Domain.Repositories
{
    public interface IMenuItemRepository : IRepository<MenuItem> { }

    public interface IMenuItemRepository<in TFilter> : IRepository<MenuItem, TFilter> { }
}
