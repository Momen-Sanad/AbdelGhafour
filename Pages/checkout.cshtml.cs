using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SuperMarket.Data;
using SuperMarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperMarket.Pages
{
    public class CheckoutModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CheckoutModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public CartViewModel Cart { get; set; } = new CartViewModel();
        public async Task<IActionResult> OnGetAsync()
        {
        
            ///ShoppingCart shoppingCart = new ShoppingCart();
            
            int? customerId = GetCustomerIdFromSession();
            if (customerId == null)
            {
                return RedirectToPage("/Account/Login");        // will we even make a login page
            }

            var shoppingCart = await _context.ShoppingCarts
                .FirstOrDefaultAsync(sc => sc.CustomerIDCart == customerId.Value);

            if (shoppingCart == null || string.IsNullOrEmpty(shoppingCart.Content))     //i have no idea why this nigga sad I NEVER STATED THAT ITS CARTVIEWMODEL
            {
                return Page();
            }

            var productQuantities = ParseContentToDictionary(shoppingCart.Content);     // same shit

            if (!productQuantities.Any())
            {
                return Page();
            }

            var productIds = productQuantities.Keys.ToList();
            var products = await _context.Products
                .Where(p => productIds.Contains(int.Parse(p.ProductID)))
                .ToListAsync();

            foreach (var product in products)
            {
                var quantity = productQuantities[int.Parse(product.ProductID)];
                var totalPrice = product.Price * quantity;

                var cartItem = new CartItem
                {
                    ProductID = int.Parse(product.ProductID),
                    Name = product.ProductName,
                    Price = product.Price,
                    Quantity = quantity,
                    TotalPrice = totalPrice,
                    ImageUrl = product.ProductImageUrl ?? ""
                };

                Cart.CartItems.Add(cartItem);
                Cart.Subtotal += totalPrice;
            }

            Cart.Tax = Cart.Subtotal * 0.10m; // 10% tax
            Cart.Total = Cart.Subtotal + Cart.Shipping + Cart.Tax;

            return Page();
        }

        public async Task<IActionResult> OnPostApplyPromoCodeAsync(string promoCode)
        {

            if (!string.IsNullOrEmpty(promoCode))
            {
                // Apply a 10% discount if the promo code is "SAVE10"
                // we will make admins create promo codes in dashboard
                // this is just an example
                if (promoCode == "SAVE10")
                {
                    Cart.Total *= 0.90m; 
                }
            }

            return Page();
        }

        private Dictionary<int, int> ParseContentToDictionary(string content)
        {
            var productQuantities = new Dictionary<int, int>();
            if (!string.IsNullOrEmpty(content))
            {
                var entries = content.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var entry in entries)
                {
                    var parts = entry.Split(':');
                    if (parts.Length == 2 &&
                        int.TryParse(parts[0], out int productId) &&
                        int.TryParse(parts[1], out int quantity))
                    {
                        productQuantities[productId] = quantity;
                    }
                }
            }
            return productQuantities;
        }



           //// dont forget to change this 
        private int? GetCustomerIdFromSession()
        {
            // replace with actual session or authentication logic
            return 1;
        }
    }
}
