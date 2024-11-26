using Waiter.Application.Security;

namespace Waiter.Application.UseCases.Users
{
    public class GetAvailableRolesUseCase
    {
        private readonly IIdentityService _identityService;

        public GetAvailableRolesUseCase(
            ITokenProvider tokenProvider,
            IIdentityService identityService
        )
        {
            _identityService = identityService;
        }

        public async Task<string[]> Get()
        {
            return await _identityService.GetRolesAsync();
        }
    }
}
