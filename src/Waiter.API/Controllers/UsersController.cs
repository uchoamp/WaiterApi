using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Waiter.Application.Exceptions;
using Waiter.Application.Models;
using Waiter.Domain.Models;

namespace Waiter.API.Controllers
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="password"></param>
    /// <param name="email"></param>
    /// <param name="firstName"></param>
    public record UserDto(string password, string email, string firstName) { }

    /// <summary>
    /// Users
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces(MediaTypeNames.Application.Json)]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        ///
        /// </summary>
        /// <param name="userManager"></param>
        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Load all users
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<UserDto> Get()
        {
            return _userManager
                .Users.Select(x => new UserDto("******", x.Email!, x.FirstName))
                .ToList();
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
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
        [ProducesResponseType<ValidationResult>(400)]
        public async Task<MessageResponse> Post([FromBody] UserDto user)
        {
            var appUser = new ApplicationUser
            {
                UserName = user.email,
                Email = user.email,
                FirstName = user.firstName,
            };

            var result = await _userManager.CreateAsync(appUser, user.password);
            if (!result.Succeeded)
            {
                throw new ValidationException(
                    result.Errors.Select(x => new ValidationItem(x.Code, x.Description)).ToArray()
                );
            }

            StatusCode(StatusCodes.Status201Created);
            return new MessageResponse("User Created!");
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value) { }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id) { }
    }
}
