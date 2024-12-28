using System.Collections.Generic;

namespace SuperMarket.Models
{
    public class CartViewModel
    {

        public int CustomerIDCart { get; set; }  
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal Subtotal { get; set; }
        public decimal Shipping { get; set; } = 5.00m;
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
    }
}