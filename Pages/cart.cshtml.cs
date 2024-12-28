using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SuperMarket.Models;
using System.Data.SqlClient;
using System.Linq;

namespace SuperMarket.Pages
{
    public class CartModel : PageModel
    {
        public Cart ShoppingCart { get; set; } = new Cart();
        public decimal Subtotal { get; private set; }
        public decimal Tax { get; private set; }
        public decimal Shipping { get; private set; }
        public decimal Total { get; private set; }

        string connectionString = "Server=127.0.0.1,1433;Database=SMS;User=SA;Password=YourStrong@Passw0rd;TrustServerCertificate=True;";

        public void OnGet()
        {
            LoadCartDetails();
            CalculateCartTotals();
        }

        public IActionResult OnPostCheckout()
        {
            LoadCartDetails();
            CalculateCartTotals();

            // Store order summary details in TempData for Payments page
            TempData["Subtotal"] = Subtotal;
            TempData["Tax"] = Tax;
            TempData["Shipping"] = Shipping;
            TempData["Total"] = Total;

            return RedirectToPage("/Payments");
        }

        private void LoadCartDetails()
        {
            ShoppingCart = new Cart();
            using SqlConnection connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();
                string query = "SELECT ProductID, ProductName, Price, SUM(Quantity) AS TotalQuantity, ItemImage FROM CartItem WHERE CartID = 1 GROUP BY ProductID, ProductName, Price, ItemImage";

                using SqlCommand command = new SqlCommand(query, connection);
                using SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    ShoppingCart.AddItem(new CartItem
                    {
                        Id = int.Parse(reader["ProductID"].ToString()),
                        Name = reader["ProductName"].ToString(),
                        Price = decimal.Parse(reader["Price"].ToString()),
                        Quantity = int.Parse(reader["TotalQuantity"].ToString()),
                        image = reader["ItemImage"].ToString()
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void CalculateCartTotals()
        {
            Subtotal = ShoppingCart.Items.Sum(item => item.Price * item.Quantity);
            Tax = Subtotal * 0.1m; // 10% tax
            Shipping = 5.00m; // Flat shipping rate
            Total = Subtotal + Tax + Shipping;
        }
    }
}
