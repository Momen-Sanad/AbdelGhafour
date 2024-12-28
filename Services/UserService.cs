using Microsoft.AspNetCore.Identity;
using SuperMarket.Models;

namespace SuperMarket.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser> AuthenticateAsync(string email, string password)
        {
            // Find the user by email
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return null; // User not found
            }

            // Check the password
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);

            if (isPasswordValid)
            {
                return user; // Authentication successful
            }

            return null; // Invalid password
        }
    }
}
