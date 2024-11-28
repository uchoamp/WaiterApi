using Waiter.Domain.Models;

namespace Waiter.Domain.Repositories
{
    public interface IRepository<TEntity>
        where TEntity : BaseEntity
    {
        public Task<IEnumerable<TEntity>> LoadAllAsync();
        public Task<TEntity> GetByIdAsync(Guid id);
        public Task<List<TEntity>> GetEntitiesWithIds(params Guid[] ids);
        public Task<PaginatedResult<TEntity>> PaginateAsync(PaginationLimits limits);
        public Task CreateAsync(TEntity entity);
        public void Update(TEntity entity);
        public void Delete(TEntity entity);
        public Task<bool> ExistsEntity(Guid id);
        public Task RefreshAsync(TEntity entity);
        public Task SaveChangesAsync();
    }

    public interface IRepository<TEntity, in TFilter> : IRepository<TEntity>
        where TEntity : BaseEntity
    {
        public Task<List<TEntity>> FilterAsync(TFilter filter);
        public Task<PaginatedResult<TEntity>> FilterAndPaginateAsync(
            TFilter filter,
            PaginationLimits limits
        );
    }
}
