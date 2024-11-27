namespace Waiter.Domain.Models
{
    public class PaginatedLimits
    {
        public int CurrentPage { get; }
        public int PageSize { get; }
        public (string, string)[]? OrderBy { get; }

        public PaginatedLimits(int currentPage, int pageSize, (string, string)[] orderBy)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            OrderBy = orderBy;
        }

        public PaginatedLimits(int currentPage, int pageSize)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
        }
    }
}
