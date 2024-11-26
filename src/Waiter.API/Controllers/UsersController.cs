using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Waiter.Application.Models.Request;
using Waiter.Application.Models.Response;
using Waiter.Application.UseCases.Users;

namespace Waiter.API.Controllers
{
    /// <summary>
    /// Users
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces(MediaTypeNames.Application.Json)]
    public class UsersController : ControllerBase
    {
        private readonly AuthorizeUserUseCase _authorizeUserUseCase;
        private readonly GetAllUsersUseCase _getAllUsersUseCase;

        /// <summary>
        ///
        /// </summary>
        /// <param name="authorizeUserUseCase"></param>
        /// <param name="getAllUsersUseCase"></param>
        public UsersController(
            AuthorizeUserUseCase authorizeUserUseCase,
            GetAllUsersUseCase getAllUsersUseCase
        )
        {
            _authorizeUserUseCase = authorizeUserUseCase;
            _getAllUsersUseCase = getAllUsersUseCase;
        }

        /// <summary>
        /// Load all users
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<UserResponse[]> Get()
        {
            return await _getAllUsersUseCase.Get();
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            var user = User.Identity.Name;
            return "value";
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType<MessageResponse>(201)]
        [ProducesResponseType<ValidationResponse>(400)]
        public async Task Post([FromBody] string name) { }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value) { }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id) { }

        /// <summary>
        /// Create access token
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns></returns>
        [HttpPost("authorize")]
        [AllowAnonymous]
        [ProducesResponseType<AccessTokenResponse>(201)]
        [ProducesResponseType<ValidationResponse>(400)]
        public async Task<AccessTokenResponse> Authorize(UserCredentialResquest credentials)
        {
            return await _authorizeUserUseCase.AuthorizeUser(credentials);
        }
    }
}
