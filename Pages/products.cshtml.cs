using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SuperMarket.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;

namespace SuperMarket.Pages
{
    [Authorize]
    public class ProductsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;  // UserManager to access user info
        public List<Product> Products { get; set; }
        public List<string> SelectedCategories { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public string SortBy { get; set; }

        public ProductsModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public string CurrentUserId { get; set; }

        public void OnGet(List<string> categories, decimal? minPrice, decimal? maxPrice, string sortBy)
        {
            CurrentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            SelectedCategories = categories ?? new List<string>();
            MinPrice = minPrice ?? 0;
            MaxPrice = maxPrice ?? 9999;
            SortBy = sortBy ?? "Popularity";

            Products = new List<Product>();

            string connectionString = "Data Source=DESKTOP-K96CGJS\\SQLEXPRESS;Initial Catalog=SMS;Integrated Security=True;TrustServerCertificate=True";
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();

                string query = "SELECT * FROM Products WHERE Price BETWEEN @MinPrice AND @MaxPrice";

                if (SelectedCategories.Count > 0)
                {
                    string categoriesPlaceholder = "";
                    foreach (var category in SelectedCategories)
                    {
                        categoriesPlaceholder += $"@Category_{category}, ";
                    }

                    if (categoriesPlaceholder.Length > 0)
                    {
                        categoriesPlaceholder = categoriesPlaceholder.Substring(0, categoriesPlaceholder.Length - 2);
                    }

                    query += $" AND Category IN ({categoriesPlaceholder})";
                }


                switch (sortBy)
                {
                    case "PriceLowToHigh":
                        query += " ORDER BY Price ASC";
                        break;
                    case "PriceHighToLow":
                        query += " ORDER BY Price DESC";
                        break;
                    case "NameAZ":
                        query += " ORDER BY ProductName ASC";
                        break;
                    default:
                        query += " ORDER BY ProductName ASC";
                        break;
                }

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@MinPrice", MinPrice);
                command.Parameters.AddWithValue("@MaxPrice", MaxPrice);

                foreach (var category in SelectedCategories)
                {
                    command.Parameters.AddWithValue($"@Category_{category}", category);
                }


                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Products.Add(new Product
                    {
                        ProductID = reader["ProductID"].ToString(),
                        ProductName = reader["ProductName"].ToString(),
                        Price = decimal.Parse(reader["Price"].ToString()),
                        Category = reader["Category"].ToString(),
                        InventoryID = reader["InventoryID"].ToString(),
                        image = reader["image"].ToString()
                    });
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                connection.Close();
            }
        }

        public IActionResult OnPostAddToCart(int productId, string productName, decimal productPrice, string productImage)
        {

            string connectionString = "Data Source=DESKTOP-K96CGJS\\SQLEXPRESS;Initial Catalog=SMS;Integrated Security=True;TrustServerCertificate=True";
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();

                string query = "INSERT INTO CartItem (CartID, CustomerID, ProductID, ProductName, ItemImage, Price, Quantity) VALUES (1, 1, @ProductID, @ProductName, @ItemImage, @Price, 1)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Set parameters to prevent SQL injection
                    command.Parameters.AddWithValue("@ProductID", productId);
                    command.Parameters.AddWithValue("@Price", productPrice);
                    command.Parameters.AddWithValue("@ProductName", productName);
                    command.Parameters.AddWithValue("@ItemImage", productImage);
                    command.ExecuteNonQuery();
                }

            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                connection.Close();
            }
            return RedirectToPage("cart");
        }
    }
    public class Product
    {
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public string InventoryID { get; set; }
        public string image { get; set; }
    }
}