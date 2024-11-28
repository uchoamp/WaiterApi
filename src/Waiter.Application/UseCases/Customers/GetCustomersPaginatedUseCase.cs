using Waiter.Application.Models.Customers;
using Waiter.Domain.Models;
using Waiter.Domain.Repositories;

namespace Waiter.Application.UseCases.Customers
{
    public class GetCustomersPaginatedUseCase
    {
        private readonly ICustomerRepository _customerRepository;

        public GetCustomersPaginatedUseCase(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<CustomersPaginatedResponse> Get(int currentPage, int pageSize)
        {
            var result = await _customerRepository.PaginateAsync(
                new PaginationLimits(currentPage, pageSize)
            );

            return new CustomersPaginatedResponse(
                currentPage,
                pageSize,
                result.LastPage,
                result
                    .Result.Select(x => new CustomerResponse(
                        x.Id,
                        x.FirstName,
                        x.LastName,
                        x.PhoneNumber
                    ))
                    .ToArray()
            );
        }
    }
}
