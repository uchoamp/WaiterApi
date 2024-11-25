using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Waiter.Application.Models;

namespace Waiter.API.Custom
{
    /// <summary>
    ///
    /// </summary>
    public class CustomAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler defaultHandler = new();

        /// <summary>
        ///
        /// </summary>
        /// <param name="next"></param>
        /// <param name="context"></param>
        /// <param name="policy"></param>
        /// <param name="authorizeResult"></param>
        /// <returns></returns>
        public async Task HandleAsync(
            RequestDelegate next,
            HttpContext context,
            AuthorizationPolicy policy,
            PolicyAuthorizationResult authorizeResult
        )
        {
            await defaultHandler.HandleAsync(next, context, policy, authorizeResult);

            if (!authorizeResult.Succeeded)
            {
                await context.Response.WriteAsJsonAsync(
                    new MessageResponse("Invalid acess token or user not authorized")
                );
            }
        }
    }
}
