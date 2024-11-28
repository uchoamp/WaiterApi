using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Waiter.Application.Models.Common;
using Waiter.Application.Models.Orders;
using Waiter.Application.UseCases.Orders;
using Waiter.Domain.Constants;
using Waiter.Domain.Enums;

namespace Waiter.API.Controllers
{
    /// <summary>
    /// Order Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Admin)]
    [Produces(MediaTypeNames.Application.Json)]
    public class OrdersController : ControllerBase
    {
        private readonly CreateOrderUseCase _createOrderUseCase;
        private readonly GetOrdersPaginatedUseCase _getOrdersPaginatedUseCase;
        private readonly GetOrderUseCase _getOrderUseCase;
        private readonly UpdateOrderUseCase _updateOrderUseCase;
        private readonly DeleteOrderUseCase _deleteOrderUseCase;
        private readonly UpdateOrderStatusUseCase _updateOrderStatusUseCase;

        /// <summary>
        ///
        /// </summary>
        /// <param name="createOrderUseCase"></param>
        /// <param name="getOrdersPaginatedUseCase"></param>
        /// <param name="getOrderUseCase"></param>
        /// <param name="updateOrderUseCase"></param>
        /// <param name="deleteOrderUseCase"></param>
        public OrdersController(
            CreateOrderUseCase createOrderUseCase,
            GetOrdersPaginatedUseCase getOrdersPaginatedUseCase,
            GetOrderUseCase getOrderUseCase,
            UpdateOrderUseCase updateOrderUseCase,
            DeleteOrderUseCase deleteOrderUseCase,
            UpdateOrderStatusUseCase updateOrderStatusUseCase
        )
        {
            _createOrderUseCase = createOrderUseCase;
            _getOrdersPaginatedUseCase = getOrdersPaginatedUseCase;
            _getOrderUseCase = getOrderUseCase;
            _updateOrderUseCase = updateOrderUseCase;
            _deleteOrderUseCase = deleteOrderUseCase;
            _updateOrderStatusUseCase = updateOrderStatusUseCase;
        }

        /// <summary>
        /// List Orders with pagination
        /// </summary>
        /// <param name="page">Current page</param>
        /// <param name="pageSize">Total itens per page</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<OrdersPaginatedResponse> Get(int page = 1, int pageSize = 10)
        {
            return await _getOrdersPaginatedUseCase.Get(page, pageSize);
        }

        /// <summary>
        /// Get Order by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType<OrderResponse>(200)]
        [ProducesResponseType<MessageResponse>(404)]
        public async Task<OrderResponse> Get(Guid id)
        {
            return await _getOrderUseCase.Get(id);
        }

        /// <summary>
        /// Create Order
        /// </summary>
        /// <param name="newOrder"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType<OrderResponse>(201)]
        [ProducesResponseType<ValidationResponse>(400)]
        public async Task<OrderResponse> Post(OrderRequest newOrder)
        {
            var orderReponse = await _createOrderUseCase.Create(newOrder);
            var locationUser = $"{Request.Scheme}://{Request.Host}{Request.Path}/{orderReponse.Id}";

            Response.Headers["Location"] = locationUser;

            return orderReponse;
        }

        /// <summary>
        /// Update Order
        /// </summary>
        /// <param name="id"></param>
        /// <param name="orderRequest"></param>
        /// <returns></returns>
        [HttpPut("{id:guid}")]
        [ProducesResponseType<OrderResponse>(200)]
        [ProducesResponseType<ValidationResponse>(400)]
        [ProducesResponseType<MessageResponse>(404)]
        public async Task<OrderResponse> Put(Guid id, OrderRequest orderRequest)
        {
            return await _updateOrderUseCase.Update(id, orderRequest);
        }

        /// <summary>
        /// Update Order Status
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPatch("{id:guid}/status")]
        [ProducesResponseType<OrderResponse>(200)]
        [ProducesResponseType<MessageResponse>(404)]
        public async Task<MessageResponse> PatchStatus(Guid id, OrderStatus status)
        {
            return await _updateOrderStatusUseCase.UpdateStatus(id, status);
        }

        /// <summary>
        /// Delete Order
        /// </summary>
        /// <param name="id">Order Id</param>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType<MessageResponse>(200)]
        public async Task<MessageResponse> Delete(Guid id)
        {
            return await _deleteOrderUseCase.Delete(id);
        }
    }
}
