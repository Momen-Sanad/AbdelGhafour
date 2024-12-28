using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SuperMarket.Pages
{
    public class AdminDashboardModel : PageModel
    {
        public List<Product> Products { get; set; } = new List<Product>();
        [BindProperty]
        public Product NewProduct { get; set; }

        public bool IsEditing => Convert.ToInt32(NewProduct?.ProductID) != 0;


        public void OnGet(int? editId)
        {
            LoadProducts();

            // Handle the edit case if `editId` is provided
            if (editId.HasValue)
            {
                NewProduct = Products.FirstOrDefault(p => Convert.ToInt32(p.ProductID) == editId.Value);
            }
            else
            {
                NewProduct = new Product(); // Initialize a new product for adding
            }
        }


        //to potentially show all products later on if needed
        public List<int> GetAllProductIDs()
        {
            var productIDs = new List<int>();
            string connectionString = "Server=127.0.0.1,1433;Database=SMS;User=SA;Password=YourStrong@Passw0rd;TrustServerCertificate=True;";

            using var connection = new SqlConnection(connectionString);
            const string query = "SELECT ProductID FROM Products";
            var command = new SqlCommand(query, connection);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (reader["ProductID"] != DBNull.Value)
                {
                    productIDs.Add(Convert.ToInt32(reader["ProductID"]));
                }
            }
            return productIDs;
        }
        public bool IsProductIDValid(int productID)
        {

            string connectionString = "Server=127.0.0.1,1433;Database=SMS;User=SA;Password=YourStrong@Passw0rd;TrustServerCertificate=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                const string query = "SELECT COUNT(1) FROM Products WHERE ProductID = @ProductID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProductID", productID);
                connection.Open();
                return (int)command.ExecuteScalar() > 0;
            }
        }



        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid)
            {
                LoadProducts();
                return Page();
            }

            int productID;
            if (!int.TryParse(NewProduct.ProductID, out productID))
            {
                ModelState.AddModelError("", "Product ID must be a valid integer.");
                LoadProducts();
                return Page();
            }

            bool isValidProductID = IsProductIDValid(productID);

            if (isValidProductID)
            {
                // ProductID already exists
                ModelState.AddModelError("", "Product ID already exists. Please use a unique Product ID.");
                LoadProducts();
                return Page();
            }

            // Proceed with inserting the new product
            string connectionString = "Server=127.0.0.1,1433;Database=SMS;User=SA;Password=YourStrong@Passw0rd;TrustServerCertificate=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO Products 
                                (ProductID, ProductName, Price, Category, InventoryID, ProductImage)
                                VALUES 
                                (@ProductID, @ProductName, @Price, @Category, @InventoryID, @ProductImage)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProductID", productID);
                command.Parameters.AddWithValue("@ProductName", NewProduct.ProductName);
                command.Parameters.AddWithValue("@Price", NewProduct.Price);
                command.Parameters.AddWithValue("@Category", NewProduct.Category);
                command.Parameters.AddWithValue("@InventoryID", NewProduct.InventoryID ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ProductImage", NewProduct.image ?? (object)DBNull.Value);

                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToPage();
        }



        public IActionResult OnPostEdit(int productId)
        {
            string connectionString = "Server=127.0.0.1,1433;Database=SMS;User=SA;Password=YourStrong@Passw0rd;TrustServerCertificate=True;";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Products SET ProductName = @ProductName, Price = @Price, category = @category, InventoryID = @InventoryID, image = @Image WHERE ProductID = @ProductID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ProductName", NewProduct.ProductName);
                    command.Parameters.AddWithValue("@Price", NewProduct.Price);
                    command.Parameters.AddWithValue("@category", NewProduct.Category);
                    command.Parameters.AddWithValue("@InventoryID", NewProduct.InventoryID);
                    command.Parameters.AddWithValue("@ProductImage", NewProduct.image);
                    command.Parameters.AddWithValue("@ProductID", productId);

                    connection.Open();
                    command.ExecuteNonQuery();
                }

                return RedirectToPage();
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
                // Optionally, handle the error (e.g., display a message to the user)
                return Page();
            }
        }

        public IActionResult OnPostDelete(string productId)
        {
            string connectionString = "Server=127.0.0.1,1433;Database=SMS;User=SA;Password=YourStrong@Passw0rd;TrustServerCertificate=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Products WHERE ProductID = @ProductID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProductID", productId);

                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToPage();
        }

        private void LoadProducts()
        {
            string connectionString = "Server=127.0.0.1,1433;Database=SMS;User=SA;Password=YourStrong@Passw0rd;TrustServerCertificate=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Products ORDER BY ProductName ASC";
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Products.Add(new Product
                    {
                        ProductID = reader["ProductID"].ToString(),
                        ProductName = reader["ProductName"].ToString(),
                        Price = decimal.Parse(reader["Price"].ToString()),
                        Category = reader["category"].ToString(),
                        InventoryID = reader["InventoryID"].ToString(),
                        image = reader["ProductImage"].ToString()
                    });
                }
            }
        }
    }
}