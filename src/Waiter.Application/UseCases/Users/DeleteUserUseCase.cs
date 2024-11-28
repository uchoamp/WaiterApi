using Waiter.Application.Models.Common;
using Waiter.Application.Security;

namespace Waiter.Application.UseCases.Users
{
    public class DeleteUserUseCase
    {
        private readonly IIdentityService _identityService;

        public DeleteUserUseCase(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<MessageResponse> Delete(Guid id)
        {
            await _identityService.DeleteUserAsync(id);

            return new MessageResponse("User has been deleted.");
        }
    }
}
