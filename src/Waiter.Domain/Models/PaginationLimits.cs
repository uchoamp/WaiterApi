namespace Waiter.Domain.Models
{
    public record PaginationLimits
    {
        public int CurrentPage { get; }
        public int PageSize { get; }
        public (string, string)[]? OrderBy { get; }

        public PaginationLimits(int currentPage, int pageSize, (string, string)[] orderBy)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            OrderBy = orderBy;
        }

        public PaginationLimits(int currentPage, int pageSize)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
        }
    }
}
