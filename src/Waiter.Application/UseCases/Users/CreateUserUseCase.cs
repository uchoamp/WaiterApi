using FluentValidation;
using Waiter.Application.Exceptions;
using Waiter.Application.Models.Users;
using Waiter.Application.Security;
using Waiter.Application.Validators;

namespace Waiter.Application.UseCases.Users
{
    public class CreateUserUseCase
    {
        private readonly IIdentityService _identityService;

        public CreateUserUseCase(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<UserResponse> Create(NewUserRequest newUser)
        {
            var validator = new NewUserRequestValidator(_identityService);
            var validationResult = await validator.ValidateAsync(newUser);

            if (!validationResult.IsValid)
            {
                throw new ApplicationValidationException(validationResult);
            }

            await _identityService.CreateUserAsync(newUser);

            var user = await _identityService.GetUserByEmailAsync(newUser.Email);

            return user!;
        }
    }
}
