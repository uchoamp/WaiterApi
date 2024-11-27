using Waiter.Domain.Models;

namespace Waiter.Domain.Repositories
{
    public interface IRepository<TEntity>
        where TEntity : BaseEntity
    {
        public Task<IEnumerable<TEntity>> LoadAllAsync();
        public Task<TEntity> GetByIdAsync(Guid id);
        public Task<PaginatedResult<TEntity>> PaginateAsync(PaginatedLimits limits);
        public Task CreateAsync(TEntity entity);
        public Task UpdateAsync(TEntity entity);
        public Task DeleteAsync(Guid id);
        public Task SaveChangesAsync();
    }

    public interface IRepository<TEntity, in TFilter> : IRepository<TEntity>
        where TEntity : BaseEntity
    {
        public Task<List<TEntity>> FilterAsync(TFilter filter);
        public Task<PaginatedResult<TEntity>> FilterAndPaginateAsync(
            TFilter filter,
            PaginatedLimits limits
        );
    }
}
