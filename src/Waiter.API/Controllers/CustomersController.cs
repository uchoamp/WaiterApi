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
        private readonly GetCustomersPaginatedUseCase _getCustomersPaginatedUseCase;
        private readonly GetCustomerUseCase _getCustomerUseCase;

        /// <summary>
        ///
        /// </summary>
        /// <param name="createCustomerUseCase"></param>
        /// <param name="getCustomersPaginatedUseCase"></param>
        /// <param name="getCustomerUseCase"></param>
        public CustomersController(
            CreateCustomerUseCase createCustomerUseCase,
            GetCustomersPaginatedUseCase getCustomersPaginatedUseCase,
            GetCustomerUseCase getCustomerUseCase
        )
        {
            _createCustomerUseCase = createCustomerUseCase;
            _getCustomersPaginatedUseCase = getCustomersPaginatedUseCase;
            _getCustomerUseCase = getCustomerUseCase;
        }

        /// <summary>
        /// List customers with pagination
        /// </summary>
        /// <param name="page">Current page</param>
        /// <param name="pageSize">Total itens per page</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<CustomersPaginatedResponse> Get(int page = 1, int pageSize = 10)
        {
            return await _getCustomersPaginatedUseCase.Get(page, pageSize);
        }

        /// <summary>
        /// Get customer by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType<CustomerResponse>(200)]
        [ProducesResponseType<MessageResponse>(404)]
        public async Task<CustomerResponse> Get(Guid id)
        {
            return await _getCustomerUseCase.Get(id);
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
