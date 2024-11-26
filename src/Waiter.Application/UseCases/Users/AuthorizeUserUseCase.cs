using Waiter.Application.Exceptions;
using Waiter.Application.Models.Request;
using Waiter.Application.Models.Response;
using Waiter.Application.Security;

namespace Waiter.Application.UseCases.Users
{
    public class AuthorizeUserUseCase
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly IIdentityService _identityService;

        public AuthorizeUserUseCase(ITokenProvider tokenProvider, IIdentityService identityService)
        {
            _tokenProvider = tokenProvider;
            _identityService = identityService;
        }

        public async Task<AccessTokenResponse> AuthorizeUser(UserCredentialResquest credentials)
        {
            var accessAthorized = await _identityService.CheckPasswordAsync(
                credentials.Email,
                credentials.Password
            );

            if (!accessAthorized)
                throw new ApplicationValidationException(
                    new ValidationItem("EmailOrPasswordInvalid", "Email or password invalid.")
                );

            var user = await _identityService.GetUserByEmailAsync(credentials.Email);

            return _tokenProvider.CreateAcessTokenAsync(user!.Id, user.Roles);
        }
    }
}
