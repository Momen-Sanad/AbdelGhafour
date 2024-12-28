using Microsoft.AspNetCore.Identity;

namespace SuperMarket.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
