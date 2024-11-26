using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Waiter.Application.Exceptions;
using Waiter.Application.Models.Request;
using Waiter.Application.Models.Response;
using Waiter.Application.Security;
using Waiter.Domain.Models;

namespace Waiter.Infra.Security
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public IdentityService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> CheckPasswordAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;

            return await _userManager.CheckPasswordAsync(user, password);
        }

        public Task CreateUserAsync(UserResquest userRequest)
        {
            throw new NotImplementedException();
        }

        public Task DeleteUserAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<string?[]> GetRolesAsync()
        {
            return await _roleManager.Roles.Select(x => x.Name).ToArrayAsync();
        }

        public Task<UserResponse> GetUserAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<UserResponse> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                throw new ValidationException(
                    new ValidationItem("UserNotFound", $"User not found for email {email}")
                );

            var roles = await _userManager.GetRolesAsync(user);

            return new UserResponse(
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email!,
                roles.ToArray()
            );
        }

        public async Task<string?[]> GetUserRolesAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                throw new ValidationException(
                    new ValidationItem("UserNotFound", "User not found.")
                );

            return (await _userManager.GetRolesAsync(user)).ToArray();
        }

        public Task<UserResponse[]> GetUsersAsync()
        {
            throw new NotImplementedException();
        }

        public Task RemoveUserRoleAsync(Guid id, string role)
        {
            throw new NotImplementedException();
        }

        public Task SetUserRoleAsync(Guid id, string role)
        {
            throw new NotImplementedException();
        }

        public Task UpdateUserAsync(UserResquest userRequest)
        {
            throw new NotImplementedException();
        }
    }
}
