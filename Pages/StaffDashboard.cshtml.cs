using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SuperMarket.Pages
{
    [Authorize(Roles ="Staff")]
    public class StaffDashboardModel : PageModel
    {
        private readonly string _connectionString;
        private readonly ILogger<StaffDashboardModel> _logger;

        public List<Product> Products { get; set; } = new List<Product>();
        [BindProperty]
        public Product NewProduct { get; set; }
        [TempData]
        public string StatusMessage { get; set; }

        public bool IsEditing => !string.IsNullOrEmpty(NewProduct?.ProductID);

        public StaffDashboardModel(IConfiguration configuration, ILogger<StaffDashboardModel> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync([FromRoute] string editId = null)
        {
            try
            {
                await LoadProductsAsync();

                if (!string.IsNullOrEmpty(editId))
                {
                    NewProduct = Products.FirstOrDefault(p => p.ProductID == editId);
                    if (NewProduct == null)
                    {
                        StatusMessage = "Error: Product not found.";
                        return RedirectToPage();
                    }
                }
                else
                {
                    NewProduct = new Product();
                }

                return Page();
            }
            catch (Exception ex)
            {
                StatusMessage = "Error loading products. Please try again.";
                return RedirectToPage();
            }
        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadProductsAsync();
                    return Page();
                }

                if (await IsProductIDExistsAsync(NewProduct.ProductID))
                {
                    ModelState.AddModelError("NewProduct.ProductID", "Product ID already exists.");
                    await LoadProductsAsync();
                    return Page();
                }

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"INSERT INTO products 
                            (ProductID, ProductName, Price, category, InventoryID, image)
                            VALUES 
                            (@ProductID, @ProductName, @Price, @Category, @InventoryID, @Image)";

                using var command = new SqlCommand(query, connection);
                AddProductParameters(command, NewProduct);
                await command.ExecuteNonQueryAsync();

                StatusMessage = "Product added successfully.";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product");
                ModelState.AddModelError("", "Error adding product. Please try again.");
                await LoadProductsAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostEditAsync([FromRoute] string editId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadProductsAsync();
                    return Page();
                }

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"UPDATE products 
                    SET ProductName = @ProductName, 
                        Price = @Price, 
                        category = @Category, 
                        InventoryID = @InventoryID, 
                        image = @Image 
                    WHERE ProductID = @ProductID";

                using var command = new SqlCommand(query, connection);
                AddProductParameters(command, NewProduct);

                int rowsAffected = await command.ExecuteNonQueryAsync();
                if (rowsAffected == 0)
                {
                    ModelState.AddModelError("", "Product not found or no changes made.");
                    await LoadProductsAsync();
                    return Page();
                }

                StatusMessage = "Product updated successfully.";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product");
                ModelState.AddModelError("", "Error updating product. Please try again.");
                await LoadProductsAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(string productId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = "DELETE FROM products WHERE ProductID = @ProductID";
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProductID", productId);

                int rowsAffected = await command.ExecuteNonQueryAsync();
                if (rowsAffected == 0)
                {
                    StatusMessage = "Error: Product not found.";
                    return RedirectToPage();
                }

                StatusMessage = "Product deleted successfully.";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product");
                StatusMessage = "Error deleting product. Please try again.";
                return RedirectToPage();
            }
        }

        private async Task LoadProductsAsync()
        {
            Products.Clear();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM products ORDER BY ProductName ASC";
            using var command = new SqlCommand(query, connection);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
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

        private async Task<bool> IsProductIDExistsAsync(string productId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT COUNT(1) FROM products WHERE ProductID = @ProductID";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProductID", productId);

            return (int)await command.ExecuteScalarAsync() > 0;
        }

        private void AddProductParameters(SqlCommand command, Product product)
        {
            command.Parameters.AddWithValue("@ProductID", product.ProductID);
            command.Parameters.AddWithValue("@ProductName", product.ProductName ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Price", product.Price);
            command.Parameters.AddWithValue("@category", product.Category ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@InventoryID", product.InventoryID ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@image", product.image ?? (object)DBNull.Value);
        }
    }
}