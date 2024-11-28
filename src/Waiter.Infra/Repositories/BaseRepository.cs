﻿using Microsoft.EntityFrameworkCore;
using Waiter.Domain.Models;
using Waiter.Domain.Repositories;
using Waiter.Infra.Data;

namespace Waiter.Infra.Repositories
{
    public abstract class BaseRepository<TEntity> : IRepository<TEntity>
        where TEntity : BaseEntity
    {
        protected DbSet<TEntity> DbSet;
        protected ApplicationDbContext DbContext;

        protected BaseRepository(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
            DbSet = DbContext.Set<TEntity>();
        }

        public async Task CreateAsync(TEntity entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            await DbSet.AddAsync(entity);
        }

        public void DeleteAsync(TEntity entity)
        {
            DbContext.Entry(entity).State = EntityState.Deleted;
        }

        public async Task<TEntity> GetByIdAsync(Guid id)
        {
            return await DbSet.FirstAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<TEntity>> LoadAllAsync()
        {
            return await DbSet.ToListAsync();
        }

        public async Task<PaginatedResult<TEntity>> PaginateAsync(PaginationLimits limits)
        {
            var baseQuery = DbSet.AsNoTracking().OrderByDescending(x => x.CreatedAt);

            var total = await baseQuery.CountAsync();

            var pageResult = await GetResultPaginated(
                limits.CurrentPage,
                limits.PageSize,
                baseQuery
            );

            return new PaginatedResult<TEntity>(
                pageResult,
                limits.CurrentPage,
                limits.PageSize,
                total
            );
        }

        public async Task SaveChangesAsync()
        {
            await DbContext.SaveChangesAsync();
        }

        public void UpdateAsync(TEntity entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            DbContext.Entry(entity).State = EntityState.Modified;
        }

        protected static async Task<List<TEntity>> GetResultPaginated(
            int currentPage,
            int pageSize,
            IQueryable<TEntity> query
        )
        {
            return await query.Skip(pageSize * (currentPage - 1)).Take(pageSize).ToListAsync();
        }

        public async Task<bool> ExistsEntity(Guid id)
        {
            return await DbSet.AnyAsync(x => x.Id == id);
        }
    }

    public abstract class BaseRepository<TEntity, TFilter>
        : BaseRepository<TEntity>,
            IRepository<TEntity, TFilter>
        where TEntity : BaseEntity
    {
        protected BaseRepository(ApplicationDbContext dbContext)
            : base(dbContext) { }

        public abstract Task<PaginatedResult<TEntity>> FilterAndPaginateAsync(
            TFilter filter,
            PaginationLimits limits
        );

        public abstract Task<List<TEntity>> FilterAsync(TFilter filter);
    }
}