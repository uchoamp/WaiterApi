using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Waiter.Application.Models.Common;
using Waiter.Application.Models.MenuItems;
using Waiter.Application.UseCases.MenuItems;
using Waiter.Domain.Constants;

namespace Waiter.API.Controllers
{
    /// <summary>
    /// MenuItem Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Admin)]
    [Produces(MediaTypeNames.Application.Json)]
    public class MenuItemsController : ControllerBase
    {
        private readonly CreateMenuItemUseCase _createMenuItemUseCase;
        private readonly GetMenuItemsPaginatedUseCase _getMenuItemsPaginatedUseCase;
        private readonly GetMenuItemUseCase _getMenuItemUseCase;
        private readonly UpdateMenuItemUseCase _updateMenuItemUseCase;
        private readonly DeleteMenuItemUseCase _deleteMenuItemUseCase;

        /// <summary>
        ///
        /// </summary>
        /// <param name="createMenuItemUseCase"></param>
        /// <param name="getMenuItemsPaginatedUseCase"></param>
        /// <param name="getMenuItemUseCase"></param>
        /// <param name="updateMenuItemUseCase"></param>
        /// <param name="deleteMenuItemUseCase"></param>
        public MenuItemsController(
            CreateMenuItemUseCase createMenuItemUseCase,
            GetMenuItemsPaginatedUseCase getMenuItemsPaginatedUseCase,
            GetMenuItemUseCase getMenuItemUseCase,
            UpdateMenuItemUseCase updateMenuItemUseCase,
            DeleteMenuItemUseCase deleteMenuItemUseCase
        )
        {
            _createMenuItemUseCase = createMenuItemUseCase;
            _getMenuItemsPaginatedUseCase = getMenuItemsPaginatedUseCase;
            _getMenuItemUseCase = getMenuItemUseCase;
            _updateMenuItemUseCase = updateMenuItemUseCase;
            _deleteMenuItemUseCase = deleteMenuItemUseCase;
        }

        /// <summary>
        /// List Menu Items with pagination
        /// </summary>
        /// <param name="page">Current page</param>
        /// <param name="pageSize">Total itens per page</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<MenuItemsPaginatedResponse> Get(int page = 1, int pageSize = 10)
        {
            return await _getMenuItemsPaginatedUseCase.Get(page, pageSize);
        }

        /// <summary>
        /// Get Menu Item by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType<MenuItemResponse>(200)]
        [ProducesResponseType<MessageResponse>(404)]
        public async Task<MenuItemResponse> Get(Guid id)
        {
            return await _getMenuItemUseCase.Get(id);
        }

        /// <summary>
        /// Create Menu Item
        /// </summary>
        /// <param name="newMenuItem"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType<MenuItemResponse>(201)]
        [ProducesResponseType<ValidationResponse>(400)]
        public async Task<MenuItemResponse> Post(MenuItemRequest newMenuItem)
        {
            var menuItemReponse = await _createMenuItemUseCase.Create(newMenuItem);
            var locationUser =
                $"{Request.Scheme}://{Request.Host}{Request.Path}/{menuItemReponse.Id}";

            Response.Headers["Location"] = locationUser;

            return menuItemReponse;
        }

        /// <summary>
        /// Update Menu Item
        /// </summary>
        /// <param name="id"></param>
        /// <param name="menuItemRequest"></param>
        /// <returns></returns>
        [HttpPut("{id:guid}")]
        [ProducesResponseType<MenuItemResponse>(200)]
        [ProducesResponseType<ValidationResponse>(400)]
        [ProducesResponseType<MessageResponse>(404)]
        public async Task<MenuItemResponse> Put(Guid id, MenuItemRequest menuItemRequest)
        {
            return await _updateMenuItemUseCase.Update(id, menuItemRequest);
        }

        /// <summary>
        /// Delete Menu Item
        /// </summary>
        /// <param name="id">MenuItem Id</param>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType<MessageResponse>(200)]
        public async Task<MessageResponse> Delete(Guid id)
        {
            return await _deleteMenuItemUseCase.Delete(id);
        }
    }
}
