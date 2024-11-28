using Microsoft.EntityFrameworkCore;
using Waiter.Application.Models.Customers;
using Waiter.Domain.Models;
using Waiter.Domain.Repositories;
using Waiter.Infra.Data;

namespace Waiter.Infra.Repositories
{
    public class CustomerRepository
        : BaseRepository<Customer, CustomerFilter>,
            ICustomerRepository<CustomerFilter>,
            ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext dbContext)
            : base(dbContext) { }

        public override async Task<PaginatedResult<Customer>> FilterAndPaginateAsync(
            CustomerFilter filter,
            PaginationLimits limits
        )
        {
            var baseQuery = DbSet
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .Where(x => x.FirstName.Contains(filter.Name) || x.LastName.Contains(filter.Name));

            var total = await baseQuery.CountAsync();

            var pageResult = await GetResultPaginated(
                limits.CurrentPage,
                limits.PageSize,
                baseQuery
            );

            return new PaginatedResult<Customer>(
                pageResult,
                limits.CurrentPage,
                limits.PageSize,
                total
            );
        }

        public override async Task<List<Customer>> FilterAsync(CustomerFilter filter)
        {
            return await DbSet
                .AsNoTracking()
                .Where(x => x.FirstName.Contains(filter.Name) || x.LastName.Contains(filter.Name))
                .ToListAsync();
        }

        public async Task<Guid> FindIdWithPhoneNumbe(string phoneNumbe)
        {
            return await DbSet
                .Where(x => x.PhoneNumber == phoneNumbe)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();
        }
    }
}
