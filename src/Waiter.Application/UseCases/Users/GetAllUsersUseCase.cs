using Waiter.Application.Models.Users;
using Waiter.Application.Security;

namespace Waiter.Application.UseCases.Users
{
    public class GetAllUsersUseCase
    {
        private readonly IIdentityService _identityService;

        public GetAllUsersUseCase(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<UserResponse[]> Get()
        {
            return await _identityService.GetUsersAsync();
        }
    }
}
