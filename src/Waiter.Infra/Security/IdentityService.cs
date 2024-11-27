using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        public async Task<UserResponse[]> GetUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();

            var usersReponse = new List<UserResponse>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                usersReponse.Add(
                    new UserResponse(
                        user.Id,
                        user.FirstName,
                        user.LastName,
                        user.Email!,
                        roles.ToArray()
                    )
                );
            }

            return usersReponse.ToArray();
        }

        public async Task CreateUserAsync(NewUserRequest userRequest)
        {
            var user = new ApplicationUser
            {
                FirstName = userRequest.FirstName,
                LastName = userRequest.LastName,
                Email = userRequest.Email,
                UserName = userRequest.Email
            };

            await _userManager.CreateAsync(user, userRequest.Password);

            await _userManager.AddToRolesAsync(user, userRequest.Roles);
        }

        public Task DeleteUserAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<HashSet<string>> GetRolesAsync()
        {
            return (await _roleManager.Roles.Where(r => r.Name != null).ToListAsync())
                .Select(r => r.Name!.ToString())
                .ToHashSet();
        }

        public async Task<UserResponse?> GetUserAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new UserResponse(
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email!,
                roles.ToArray()
            );
        }

        public async Task<UserResponse?> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new UserResponse(
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email!,
                roles.ToArray()
            );
        }

        public async Task<HashSet<string>?> GetUserRolesAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                return null;

            return (await _userManager.GetRolesAsync(user)).ToHashSet();
        }

        public Task RemoveUserRoleAsync(Guid id, string role)
        {
            throw new NotImplementedException();
        }

        public Task SetUserRoleAsync(Guid id, string role)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateUserAsync(UpdateUserRequest modifiedUser)
        {
            var user = await _userManager.FindByIdAsync(modifiedUser.Id.ToString());

            if (user == null)
                return;

            user.UserName = modifiedUser.Email;
            user.Email = modifiedUser.Email;
            user.FirstName = modifiedUser.FirstName;
            user.LastName = modifiedUser.LastName;

            await _userManager.UpdateAsync(user);
        }

        public async Task<Guid?> GetUserIdWithEmail(string email)
        {
            return (await _userManager.FindByEmailAsync(email))?.Id;
        }
    }
}
