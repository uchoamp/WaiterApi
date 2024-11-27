using System.Security.Claims;
using Waiter.Application.Interfaces;

namespace Waiter.API.Services
{
    /// <summary>
    ///
    /// </summary>
    public class CurrentUser : IUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        ///
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Get the Id of the current authenticated user
        /// </summary>
        public Guid? Id
        {
            get
            {
                var stringId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(
                    ClaimTypes.NameIdentifier
                );

                if (string.IsNullOrEmpty(stringId))
                {
                    return null;
                }

                return new Guid(stringId);
            }
        }
    }
}
