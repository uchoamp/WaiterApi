using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Waiter.Application.Models.Common;
using Waiter.Application.Models.CustomerOrders;
using Waiter.Application.Models.Orders;
using Waiter.Application.UseCases.CustomerOrders;
using Waiter.Domain.Constants;

namespace Waiter.API.Controllers
{
    /// <summary>
    /// Customer Orders Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    [Produces(MediaTypeNames.Application.Json)]
    public class CustomerOrdersController : ControllerBase
    {
        private readonly CreateCustomerOrderUseCase _createCustomerOrderUseCase;

        /// <summary>
        ///
        /// </summary>
        /// <param name="createCustomerOrderUseCase"></param>
        public CustomerOrdersController(CreateCustomerOrderUseCase createCustomerOrderUseCase)
        {
            _createCustomerOrderUseCase = createCustomerOrderUseCase;
        }

        /// <summary>
        /// Direct customer order creation
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType<OrderResponse>(201)]
        [ProducesResponseType<ValidationResponse>(400)]
        public async Task<OrderResponse> Post(CustomerOrderRequest request)
        {
            var orderReponse = await _createCustomerOrderUseCase.Create(request);
            var locationUser = $"{Request.Scheme}://{Request.Host}{Request.Path}/{orderReponse.Id}";

            Response.Headers["Location"] = locationUser;

            return orderReponse;
        }
    }
}
