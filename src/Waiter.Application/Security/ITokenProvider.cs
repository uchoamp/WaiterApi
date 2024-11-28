using Waiter.Application.Models.Common;

namespace Waiter.Application.Security
{
    public interface ITokenProvider
    {
        AccessTokenResponse CreateAcessTokenAsync(Guid userId, string?[] roles);
    }
}
