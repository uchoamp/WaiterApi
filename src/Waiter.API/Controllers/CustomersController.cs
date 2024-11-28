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
        private readonly UpdateCustomerUseCase _updateCustomerUseCase;
        private readonly DeleteCustomerUseCase _deleteCustomerUseCase;

        /// <summary>
        ///
        /// </summary>
        /// <param name="createCustomerUseCase"></param>
        /// <param name="getCustomersPaginatedUseCase"></param>
        /// <param name="getCustomerUseCase"></param>
        /// <param name="updateCustomerUseCase"></param>
        /// <param name="deleteCustomerUseCase"></param>
        public CustomersController(
            CreateCustomerUseCase createCustomerUseCase,
            GetCustomersPaginatedUseCase getCustomersPaginatedUseCase,
            GetCustomerUseCase getCustomerUseCase,
            UpdateCustomerUseCase updateCustomerUseCase,
            DeleteCustomerUseCase deleteCustomerUseCase
        )
        {
            _createCustomerUseCase = createCustomerUseCase;
            _getCustomersPaginatedUseCase = getCustomersPaginatedUseCase;
            _getCustomerUseCase = getCustomerUseCase;
            _updateCustomerUseCase = updateCustomerUseCase;
            _deleteCustomerUseCase = deleteCustomerUseCase;
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
            var customerReponse = await _createCustomerUseCase.Create(newCustomer);
            var locationUser =
                $"{Request.Scheme}://{Request.Host}{Request.Path}/{customerReponse.Id}";

            Response.Headers["Location"] = locationUser;

            return customerReponse;
        }

        /// <summary>
        /// Update customer
        /// </summary>
        /// <param name="id"></param>
        /// <param name="customerRequest"></param>
        /// <returns></returns>
        [HttpPut("{id:guid}")]
        [ProducesResponseType<CustomerResponse>(200)]
        [ProducesResponseType<ValidationResponse>(400)]
        [ProducesResponseType<MessageResponse>(404)]
        public async Task<CustomerResponse> Put(Guid id, CustomerRequest customerRequest)
        {
            return await _updateCustomerUseCase.Update(id, customerRequest);
        }

        /// <summary>
        /// Delete a customer
        /// </summary>
        /// <param name="id">Customer Id</param>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType<MessageResponse>(200)]
        public async Task<MessageResponse> Delete(Guid id)
        {
            return await _deleteCustomerUseCase.Delete(id);
        }
    }
}
