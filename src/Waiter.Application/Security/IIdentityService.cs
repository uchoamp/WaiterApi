using Waiter.Application.Models.Request;
using Waiter.Application.Models.Response;

namespace Waiter.Application.Security
{
    public interface IIdentityService
    {
        Task CreateUserAsync(UserRequest userRequest);
        Task UpdateUserAsync(UserRequest userRequest);
        Task DeleteUserAsync(Guid id);
        Task<UserResponse[]> GetUsersAsync();
        Task<UserResponse> GetUserAsync(Guid id);
        Task<UserResponse?> GetUserByEmailAsync(string email);
        Task<Guid?> GetUserIdWithEmail(string email);
        Task<bool> CheckPasswordAsync(string email, string password);
        Task<HashSet<string>?> GetUserRolesAsync(Guid id);
        Task<HashSet<string>> GetRolesAsync();
        Task SetUserRoleAsync(Guid id, string role);
        Task RemoveUserRoleAsync(Guid id, string role);
    }
}
