using Waiter.Domain.Models;

namespace Waiter.Domain.Repositories
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Guid> FindIdWithPhoneNumber(string phoneNumbe);
    }

    public interface ICustomerRepository<in TFilter> : IRepository<Customer, TFilter> { }
}
