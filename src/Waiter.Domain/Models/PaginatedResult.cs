﻿namespace Waiter.Domain.Models
{
    public class PaginatedResult<TEntity>
        where TEntity : BaseEntity
    {
        public List<TEntity> Result { get; }
        public int CurrentPage { get; }
        public int PageSize { get; }
        public int TotalEntities { get; }
        public int LastPage =>
            PageSize == 0 ? 1 : (int)Math.Ceiling((double)TotalEntities / PageSize);

        public PaginatedResult(
            List<TEntity> result,
            int currentPage,
            int pageSize,
            int totalEntities
        )
        {
            Result = result;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalEntities = totalEntities;
        }
    }
}
