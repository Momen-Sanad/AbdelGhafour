using Microsoft.EntityFrameworkCore;
using SuperMarket.Models;

namespace SuperMarket.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {   
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<CartViewModel> ShoppingCarts { get; set; }
        // Include other DbSets as needed
    }
}