using Waiter.Application.Exceptions;
using Waiter.Application.Models.Response;
using Waiter.Application.Security;

namespace Waiter.Application.UseCases.Users
{
    public class GetUserUseCase
    {
        private readonly IIdentityService _identityService;

        public GetUserUseCase(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<UserResponse> Get(Guid id)
        {
            var userResponse = await _identityService.GetUserAsync(id);

            if (userResponse == null)
            {
                throw new ResourceNotFoundException("User");
            }

            return userResponse;
        }
    }
}
