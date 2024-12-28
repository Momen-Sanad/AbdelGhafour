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
    public class CartModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private const decimal TaxRate = 0.10m;

        public CartModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public CartViewModel Cart { get; set; } = new CartViewModel();

        public async Task<IActionResult> OnGetAsync()
        {
            int? customerId = GetCustomerIdFromSession();
            if (customerId == null)
            {
                return RedirectToPage("/Account/Login");
            }

            Cart.CustomerIDCart = customerId.Value;

            var shoppingCart = await GetShoppingCartAsync(customerId.Value);
            if (shoppingCart == null || string.IsNullOrEmpty(shoppingCart.Content))
            {
                return Page();
            }

            var productQuantities = ParseCartContent(shoppingCart.Content);
            if (!productQuantities.Any())
            {
                return Page();
            }

            var products = await GetProductsAsync(productQuantities.Keys.ToList());
            PopulateCart(products, productQuantities);

            return Page();
        }

        // Retrieve the shopping cart for a specific customer
        private async Task<ShoppingCart> GetShoppingCartAsync(int customerId)
        {
            return await _context.ShoppingCarts
                .AsNoTracking()
                .FirstOrDefaultAsync(cart => cart.CustomerID == customerId);    // retardism
        }
        private async Task<List<Product>> GetProductsAsync(List<int> productIds)
        {
            return await _context.Products
                .Where(product => productIds.Contains(int.Parse(product.ProductID)))
                .ToListAsync();
        }

        private void PopulateCart(List<Product> products, Dictionary<int, int> quantities)
        {
            Cart.Subtotal = 0;

            foreach (var product in products)
            {   
                int quantity = quantities[int.Parse(product.ProductID)];
                decimal totalPrice = product.Price * quantity;

                var cartItem = new CartItem
                {
                    ProductID = int.Parse(product.ProductID),
                    Name = product.ProductName,
                    Price = product.Price,
                    Quantity = quantity,
                    TotalPrice = totalPrice,
                    ImageUrl = product.ProductImageUrl ?? string.Empty
                };

                Cart.CartItems.Add(cartItem);
                Cart.Subtotal += totalPrice;
            }

            // Calculate tax and total
            Cart.Tax = Cart.Subtotal * TaxRate;
            Cart.Total = Cart.Subtotal + Cart.Shipping + Cart.Tax;
        }

        private void UpdateProductQuantity(Dictionary<int, int> quantities, int productId, int quantityChange)
        {
            if (quantities.ContainsKey(productId))
            {
                int newQuantity = quantities[productId] + quantityChange;
                if (newQuantity > 0)
                {
                    quantities[productId] = newQuantity;
                }
                else
                {
                    quantities.Remove(productId);
                }
            }
            else if (quantityChange > 0)
            {
                quantities[productId] = quantityChange;
            }
        }

         private ShoppingCart CreateNewCart(int customerId)
        {
            var newCart = new ShoppingCart
            {
                CustomerID = customerId,
                CreationDate = DateTime.UtcNow,
                Content = string.Empty
            };

            _context.Add(newCart);
            return newCart;
        }
        public async Task<IActionResult> UpdateQuantityAsync(int productId, int quantityChange)
        {
            int? customerId = GetCustomerIdFromSession();
            if (customerId == null)
            {
                return RedirectToPage("/Account/Login");
            }

            var cart = await GetShoppingCartAsync(customerId.Value) ?? CreateNewCart(customerId.Value);
            var productQuantities = ParseCartContent(cart.Content);

            UpdateProductQuantity(productQuantities, productId, quantityChange);

            cart.Content = SerializeCartContent(productQuantities);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        private string SerializeCartContent(Dictionary<int, int> quantities)
        {
            // serializing the dictionary to a string so i can use it easier later
            return string.Empty;
        }

        public async Task<IActionResult> OnPostRemoveItemAsync(int productId)
        {
            // this section should probably be a standalone function
            int? customerId = GetCustomerIdFromSession();
            if (customerId == null)
            {
                return RedirectToPage("/Account/Login");
            }
            // till here ^^

            var cart = await GetShoppingCartAsync(customerId.Value);
            if (cart != null && !string.IsNullOrEmpty(cart.Content))
            {
                var productQuantities = ParseCartContent(cart.Content);
                if (productQuantities.ContainsKey(productId))
                {
                    productQuantities.Remove(productId);
                    cart.Content = SerializeCartContent(productQuantities);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostIncreaseQuantityAsync(int id)
        {
            return await UpdateQuantityAsync(id, 1);
        }

        public async Task<IActionResult> OnPostDecreaseQuantityAsync(int id)
        {
            return await UpdateQuantityAsync(id, -1);
        }

        

        private Dictionary<int, int> ParseCartContent(string content)
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

        private string SerializeDictionaryToContent(Dictionary<int, int> productQuantities)
        {
            return string.Join(",", productQuantities.Select(kvp => $"{kvp.Key}:{kvp.Value}"));
        }

        private int? GetCustomerIdFromSession()
        {
            // Replace with actual session management
            return 1;
        }
    }
}