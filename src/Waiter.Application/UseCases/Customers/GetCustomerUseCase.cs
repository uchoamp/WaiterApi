using Waiter.Application.Exceptions;
using Waiter.Application.Models.Customers;
using Waiter.Domain.Models;
using Waiter.Domain.Repositories;

namespace Waiter.Application.UseCases.Customers
{
    public class GetCustomerUseCase
    {
        private readonly ICustomerRepository _customerRepository;

        public GetCustomerUseCase(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<CustomerResponse> Get(Guid id)
        {
            if (!await _customerRepository.ExistsEntity(id))
                throw new ResourceNotFoundException("Customer");

            var customer = await _customerRepository.GetByIdAsync(id);

            return new CustomerResponse(
                customer.Id,
                customer.FirstName,
                customer.LastName,
                customer.PhoneNumber
            );
        }
    }
}
