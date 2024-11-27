using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Waiter.Application.Models.Request;
using Waiter.Application.Models.Response;
using Waiter.Application.UseCases.Users;
using Waiter.Domain.Constants;

namespace Waiter.API.Controllers
{
    /// <summary>
    /// Users Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces(MediaTypeNames.Application.Json)]
    public class UsersController : ControllerBase
    {
        private readonly AuthorizeUserUseCase _authorizeUserUseCase;
        private readonly GetAllUsersUseCase _getAllUsersUseCase;
        private readonly GetAvailableRolesUseCase _getAvailableRolesUseCase;
        private readonly CreateUserUseCase _createUserUseCase;
        private readonly GetUserUseCase _getUserUseCase;
        private readonly UpdateUserUseCase _updateUserUseCase;
        private readonly DeleteUserUseCase _deleteUserUseCase;

        /// <summary>
        ///
        /// </summary>
        /// <param name="authorizeUserUseCase"></param>
        /// <param name="getAllUsersUseCase"></param>
        /// <param name="getAvailableRolesUseCase"></param>
        /// <param name="createUserUseCase"></param>
        /// <param name="getUserUseCase"></param>
        /// <param name="updateUserUseCase"></param>
        /// <param name="deleteUserUseCase"></param>
        public UsersController(
            AuthorizeUserUseCase authorizeUserUseCase,
            GetAllUsersUseCase getAllUsersUseCase,
            GetAvailableRolesUseCase getAvailableRolesUseCase,
            CreateUserUseCase createUserUseCase,
            GetUserUseCase getUserUseCase,
            UpdateUserUseCase updateUserUseCase,
            DeleteUserUseCase deleteUserUseCase
        )
        {
            _authorizeUserUseCase = authorizeUserUseCase;
            _getAllUsersUseCase = getAllUsersUseCase;
            _getAvailableRolesUseCase = getAvailableRolesUseCase;
            _createUserUseCase = createUserUseCase;
            _getUserUseCase = getUserUseCase;
            _updateUserUseCase = updateUserUseCase;
            _deleteUserUseCase = deleteUserUseCase;
        }

        /// <summary>
        /// Load all users
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = Roles.Admin)]
        public async Task<UserResponse[]> Get()
        {
            return await _getAllUsersUseCase.Get();
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
            return await _getUserUseCase.Get(id);
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
            var userResponse = await _createUserUseCase.Create(userRequset);

            var locationUser = $"{Request.Scheme}://{Request.Host}{Request.Path}/{userResponse.Id}";

            Response.Headers["Location"] = locationUser;

            return userResponse;
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
            return await _updateUserUseCase.Update(
                modifiedUser ?? new UpdateUserRequest(Guid.Empty, "", "", "")
            );
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
            return await _deleteUserUseCase.Delete(id);
        }

        /// <summary>
        /// Retrieve roles available
        /// </summary>
        /// <returns>List with the roles</returns>
        [HttpGet("available-roles")]
        [ProducesResponseType<HashSet<string>>(200)]
        [ProducesResponseType<ValidationResponse>(400)]
        [Authorize(Roles = Roles.Admin)]
        public async Task<HashSet<string>> AvailableRoles()
        {
            return await _getAvailableRolesUseCase.Get();
        }

        /// <summary>
        /// Create access token
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns></returns>
        [HttpPost("authorize")]
        [AllowAnonymous]
        [ProducesResponseType<AccessTokenResponse>(200)]
        [ProducesResponseType<ValidationResponse>(400)]
        public async Task<AccessTokenResponse> Authorize(UserCredentialResquest credentials)
        {
            return await _authorizeUserUseCase.AuthorizeUser(credentials);
        }
    }
}
