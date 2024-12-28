using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SuperMarket.Data;
using SuperMarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperMarket.Pages
{
    public class PaymentsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public PaymentsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string CardName { get; set; }

        [BindProperty]
        public string CardNumber { get; set; }

        [BindProperty]
        public string ExpiryDate { get; set; }

        [BindProperty]
        public string CVV { get; set; }

        [BindProperty]
        public string BillingAddress { get; set; }

        public Payments PaymentDetails { get; set; } = new Payments();

        public decimal Subtotal { get; set; }
        public decimal Shipping { get; set; } = 5.00m;
        public decimal Tax { get; set; }
        public decimal Total { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Load the shopping cart for the current customer
            int? customerId = GetCustomerIdFromSession();
            if (customerId == null)
            {
                return RedirectToPage("/Account/Login");
            }

            var shoppingCart = await _context.ShoppingCarts
                .FirstOrDefaultAsync(sc => sc.CustomerID == customerId.Value);  //why tf is this sad ?? its wokring in cart.cshtml.cs

            if (shoppingCart == null || string.IsNullOrEmpty(shoppingCart.Content))
            {
                return Page();
            }

            var productQuantities = ParseContentToDictionary(shoppingCart.Content);

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
                var quantity = productQuantities[product.ProductID];
                var totalPrice = product.Price * quantity;

                var paymentItem = new PaymentsItem    //trying to refrence all cart items here
                {                                     // so i need to join CartViews and shoppingCart on their ID 
                    Id = product.ProductID,           //then access all items of CartViews of that cartID
                    Name = product.ProductName,
                    Price = product.Price,
                    Quantity = quantity
                };

                PaymentDetails.AddItem(paymentItem);
                Subtotal += totalPrice;
            }

            Tax = Subtotal * 0.10m; // Example: 10% tax
            Total = Subtotal + Shipping + Tax;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Process payment logic here
            // For example, save payment details to the database or call a payment gateway API

            // Clear the shopping cart after successful payment
            int? customerId = GetCustomerIdFromSession();
            if (customerId != null)
            {
                var shoppingCart = await _context.ShoppingCarts
                    .FirstOrDefaultAsync(sc => sc.CustomerID == customerId.Value);

                if (shoppingCart != null)
                {
                    shoppingCart.Content = string.Empty;
                    await _context.SaveChangesAsync();
                }
            }

            // Redirect to a confirmation page or display a success message
            return RedirectToPage("/OrderConfirmation");
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


///////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //dont forget to change this 

        private int? GetCustomerIdFromSession()
        {
            // replace with  actual session or authentication logic (if we do login aslan)
            return 1;
        }
    }
}