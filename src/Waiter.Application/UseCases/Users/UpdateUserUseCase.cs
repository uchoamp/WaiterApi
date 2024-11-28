using Waiter.Application.Exceptions;
using Waiter.Application.Models.Users;
using Waiter.Application.Security;
using Waiter.Application.Validators.Users;

namespace Waiter.Application.UseCases.Users
{
    public class UpdateUserUseCase
    {
        private readonly IIdentityService _identityService;

        public UpdateUserUseCase(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<UserResponse> Update(UpdateUserRequest modifiedUser)
        {
            var validator = new UpdateUserRequestValidator(_identityService);
            var validationResult = await validator.ValidateAsync(modifiedUser);

            if (!validationResult.IsValid)
            {
                throw new ApplicationValidationException(validationResult);
            }

            await _identityService.UpdateUserAsync(modifiedUser);

            var user = await _identityService.GetUserAsync(modifiedUser.Id);

            return user!;
        }
    }
}
