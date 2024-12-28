using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace SuperMarket.Pages
{
    public class AdminDashboardModel : PageModel
    {
        public List<Product> Products { get; set; } = new List<Product>();
        [BindProperty]
        public Product NewProduct { get; set; }

        public bool IsEditing => NewProduct?.ProductID != null && Convert.ToInt32(NewProduct.ProductID) != 0;

        public void OnGet(int? editId)
        {
            LoadProducts();

            if (editId.HasValue)
            {
                NewProduct = Products.FirstOrDefault(p => Convert.ToInt32(p.ProductID) == editId.Value);
            }
            else
            {
                NewProduct = new Product(); // Initialize a new product for adding
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
                ModelState.AddModelError("", "Product ID already exists. Please use a unique Product ID.");
                LoadProducts();
                return Page();
            }

            string connectionString = "Data Source=DESKTOP-K96CGJS\\SQLEXPRESS;Initial Catalog=SMS;Integrated Security=True;TrustServerCertificate=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO Products 
                                (ProductID, ProductName, Price, category, InventoryID, image)
                                VALUES 
                                (@ProductID, @ProductName, @Price, @Category, @InventoryID, @Image)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProductID", productID);
                command.Parameters.AddWithValue("@ProductName", NewProduct.ProductName);
                command.Parameters.AddWithValue("@Price", NewProduct.Price);
                command.Parameters.AddWithValue("@Category", NewProduct.Category);
                command.Parameters.AddWithValue("@InventoryID", NewProduct.InventoryID);
                command.Parameters.AddWithValue("@Image", NewProduct.image);

                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostEdit(int productId)
        {
            if (!ModelState.IsValid)
            {
                LoadProducts();
                return Page();
            }

            string connectionString = "Data Source=DESKTOP-K96CGJS\\SQLEXPRESS;Initial Catalog=SMS;Integrated Security=True;TrustServerCertificate=True";

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
                    command.Parameters.AddWithValue("@Image", NewProduct.image);
                    command.Parameters.AddWithValue("@ProductID", productId);

                    connection.Open();
                    command.ExecuteNonQuery();
                }

                return RedirectToPage();
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
                return Page();
            }
        }

        public IActionResult OnPostDelete(string productId)
        {
            string connectionString = "Data Source=DESKTOP-K96CGJS\\SQLEXPRESS;Initial Catalog=SMS;Integrated Security=True;TrustServerCertificate=True";

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
            string connectionString = "Data Source=DESKTOP-K96CGJS\\SQLEXPRESS;Initial Catalog=SMS;Integrated Security=True;TrustServerCertificate=True";

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
                        image = reader["image"].ToString()
                    });
                }
            }
        }

        private bool IsProductIDValid(int productID)
        {
            string connectionString = "Data Source=DESKTOP-K96CGJS\\SQLEXPRESS;Initial Catalog=SMS;Integrated Security=True;TrustServerCertificate=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                const string query = "SELECT COUNT(1) FROM Products WHERE ProductID = @ProductID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProductID", productID);
                connection.Open();
                return (int)command.ExecuteScalar() > 0;
            }
        }
    }
}
