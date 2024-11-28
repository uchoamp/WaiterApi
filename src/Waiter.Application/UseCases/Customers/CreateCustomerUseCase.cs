using Waiter.Application.Exceptions;
using Waiter.Application.Models.Customers;
using Waiter.Application.Validators.Customers;
using Waiter.Domain.Models;
using Waiter.Domain.Repositories;

namespace Waiter.Application.UseCases.Customers
{
    public class CreateCustomerUseCase
    {
        private readonly ICustomerRepository _customerRepository;

        public CreateCustomerUseCase(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<CustomerResponse> Create(CustomerRequest newCustomer)
        {
            var validator = new CustomerRequestValidator(Guid.Empty, _customerRepository);
            var validationResult = await validator.ValidateAsync(newCustomer);

            if (!validationResult.IsValid)
            {
                throw new ApplicationValidationException(validationResult);
            }

            var customerEntity = new Customer
            {
                FirstName = newCustomer.FirstName,
                LastName = newCustomer.LastName,
                PhoneNumber = newCustomer.PhoneNumber.RemoveMask(),
            };

            await _customerRepository.CreateAsync(customerEntity);
            await _customerRepository.SaveChangesAsync();

            return new CustomerResponse(
                customerEntity.Id,
                customerEntity.FirstName,
                customerEntity.LastName,
                customerEntity.PhoneNumber
            );
        }
    }
}
