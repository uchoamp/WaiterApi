using Waiter.Application.Models.Common;
using Waiter.Domain.Repositories;

namespace Waiter.Application.UseCases.Customers
{
    public class DeleteCustomerUseCase
    {
        private readonly ICustomerRepository _customerRepository;

        public DeleteCustomerUseCase(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<MessageResponse> Delete(Guid id)
        {
            if (await _customerRepository.ExistsEntity(id))
            {
                var customer = await _customerRepository.GetByIdAsync(id);

                _customerRepository.Delete(customer);

                await _customerRepository.SaveChangesAsync();
            }

            return new MessageResponse("Customer has been deleted.");
        }
    }
}
