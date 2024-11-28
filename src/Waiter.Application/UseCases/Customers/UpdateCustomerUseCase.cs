using Waiter.Application.Exceptions;
using Waiter.Application.Models.Customers;
using Waiter.Application.Validators.Customers;
using Waiter.Domain.Repositories;

namespace Waiter.Application.UseCases.Customers
{
    public class UpdateCustomerUseCase
    {
        private readonly ICustomerRepository _customerRepository;

        public UpdateCustomerUseCase(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<CustomerResponse> Update(Guid id, CustomerRequest customerRequest)
        {
            if (!await _customerRepository.ExistsEntity(id))
                throw new ResourceNotFoundException("Customer");

            var validator = new CustomerRequestValidator(id, _customerRepository);
            var validationResult = await validator.ValidateAsync(customerRequest);

            if (!validationResult.IsValid)
                throw new ApplicationValidationException(validationResult);

            var customer = await _customerRepository.GetByIdAsync(id);

            customer.FirstName = customerRequest.FirstName;
            customer.LastName = customerRequest.LastName;
            customer.PhoneNumber = customerRequest.PhoneNumber.RemoveMask();

            _customerRepository.Update(customer);
            await _customerRepository.SaveChangesAsync();

            return new CustomerResponse(
                customer.Id,
                customer.FirstName,
                customer.LastName,
                customer.PhoneNumber
            );
        }
    }
}
