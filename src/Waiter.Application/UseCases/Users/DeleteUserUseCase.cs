using Waiter.Application.Exceptions;
using Waiter.Application.Models.Request;
using Waiter.Application.Models.Response;
using Waiter.Application.Security;
using Waiter.Application.Validators;

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

            return new MessageResponse("User has been deleted");
        }
    }
}
