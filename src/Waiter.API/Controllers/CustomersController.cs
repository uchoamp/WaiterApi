using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Waiter.Application.Models.Common;
using Waiter.Application.Models.Users;
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
        public CustomersController() { }

        /// <summary>
        /// Load all users
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<UserResponse[]> Get()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get user by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType<UserResponse>(200)]
        [ProducesResponseType<MessageResponse>(404)]
        public async Task<UserResponse> Get(Guid id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create a user with roles
        /// </summary>
        /// <param name="userRequset"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType<UserResponse>(201)]
        [ProducesResponseType<ValidationResponse>(400)]
        public async Task<UserResponse> Post(NewUserRequest userRequset)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update user details
        /// </summary>
        /// <param name="modifiedUser"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType<UserResponse>(200)]
        [ProducesResponseType<ValidationResponse>(400)]
        public async Task<UserResponse> Put(UpdateUserRequest modifiedUser)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete a user
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
