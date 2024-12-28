using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SuperMarket.Models;
using SuperMarket.Pages;


namespace SuperMarket.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>  
    {
        public DbSet<Customer> Customers { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ShoppingCart>()
                .Property(c => c.CreationDate)
                .HasColumnType("date");
        }
    }
}
