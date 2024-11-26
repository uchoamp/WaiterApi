using Waiter.Application.Models.Response;

namespace Waiter.Application.Security
{
    public interface ITokenProvider
    {
        Task<AccessTokenResponse> CreateAcessTokenAsync(Guid userId, string[] roles);
    }
}
