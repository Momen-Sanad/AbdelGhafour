using SuperMarket.Models;

namespace SuperMarket.Services
{
    public interface IUserService
    {
        Task<ApplicationUser> AuthenticateAsync(string email, string password);
    }
}
