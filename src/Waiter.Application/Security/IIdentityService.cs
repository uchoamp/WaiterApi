using Waiter.Application.Models.Request;
using Waiter.Application.Models.Response;

namespace Waiter.Application.Security
{
    public interface IIdentityService
    {
        Task CreateUserAsync(UserResquest userRequest);
        Task UpdateUserAsync(UserResquest userRequest);
        Task DeleteUserAsync(Guid id);
        Task<UserResponse> GetUserAsync(Guid id);
        Task<UserResponse> GetUserByEmailAsync(string email);
        Task<bool> CheckPasswordAsync(string email, string password);
        Task<string[]> GetUserRolesAsync(Guid id);
        Task<string[]> GetRolesAsync();
        Task SetUserRoleAsync(Guid id, string role);
        Task RemoveUserRoleAsync(Guid id, string role);
    }
}
