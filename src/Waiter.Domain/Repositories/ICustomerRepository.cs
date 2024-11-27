using Waiter.Domain.Models;

namespace Waiter.Domain.Repositories
{
    public interface ICustomerRepository<in TFilter> : IRepository<Customer, TFilter> { }
}
