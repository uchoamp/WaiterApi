using Waiter.Application.Security;

namespace Waiter.Application.UseCases.Users
{
    public class GetAvailableRolesUseCase
    {
        private readonly IIdentityService _identityService;

        public GetAvailableRolesUseCase(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<HashSet<string>> Get()
        {
            return await _identityService.GetRolesAsync();
        }
    }
}
