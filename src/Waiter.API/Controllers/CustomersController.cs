using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Waiter.Application.Models.Common;
using Waiter.Application.Models.Customers;
using Waiter.Application.UseCases.Customers;
using Waiter.Domain.Constants;

namespace Waiter.API.Controllers
{
    /// <summary>
    /// Customer Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Admin)]
    [Produces(MediaTypeNames.Application.Json)]
    public class CustomersController : ControllerBase
    {
        private readonly CreateCustomerUseCase _createCustomerUseCase;

        /// <summary>
        ///
        /// </summary>
        /// <param name="createCustomerUseCase"></param>
        public CustomersController(CreateCustomerUseCase createCustomerUseCase)
        {
            _createCustomerUseCase = createCustomerUseCase;
        }

        /// <summary>
        /// List customers with pagination
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<CustomerResponse[]> Get()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get customer by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType<CustomerResponse>(200)]
        [ProducesResponseType<MessageResponse>(404)]
        public async Task<CustomerResponse> Get(Guid id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create a customer
        /// </summary>
        /// <param name="newCustomer"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType<CustomerResponse>(201)]
        [ProducesResponseType<ValidationResponse>(400)]
        public async Task<CustomerResponse> Post(CustomerRequest newCustomer)
        {
            return await _createCustomerUseCase.Create(newCustomer);
        }

        /// <summary>
        /// Update customer
        /// </summary>
        /// <param name="updateCustomer"></param>
        /// <returns></returns>
        [HttpPut("{id:guid}")]
        [ProducesResponseType<CustomerResponse>(200)]
        [ProducesResponseType<ValidationResponse>(400)]
        public async Task<CustomerResponse> Put(Guid id, CustomerRequest updateCustomer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete a customer
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType<MessageResponse>(200)]
        public async Task<MessageResponse> Delete(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
