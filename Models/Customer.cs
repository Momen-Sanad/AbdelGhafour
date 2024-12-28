namespace SuperMarket.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        // Foreign Key to ApplicationUser
        public string ApplicationUserId { get; set; }

        public string CustomerPassword { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPhone { get; set; }

        // Navigation Property to ApplicationUser
        public ApplicationUser User { get; set; }

        // Navigation properties to other related entities
        public ICollection<Order> Orders { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
    }
}
