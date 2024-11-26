using Waiter.Domain.Models;

namespace Waiter.Domain.Repositories
{
    public interface IRepository<T>
        where T : BaseEntity
    {
        public Task<IEnumerable<T>> LoadAllAsync();
        public Task GetByIdAsync(Guid id);
        public Task CreateAsync(T entity);
        public Task UpdateAsync(T entity);
        public Task SaveChangesAsync();
    }
}
