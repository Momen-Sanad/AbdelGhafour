using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using SuperMarket.Models;

namespace SuperMarket.Pages
{
    public class ProductsModel : PageModel
    {
        public List<Product> Products { get; set; }
        public List<string> SelectedCategories { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public string SortBy { get; set; }

        public void OnGet(List<string> categories, decimal? minPrice, decimal? maxPrice, string sortBy)
        {
            SelectedCategories = categories ?? new List<string>();
            MinPrice = minPrice ?? 0;
            MaxPrice = maxPrice ?? 9999;
            SortBy = sortBy ?? "Popularity";

            Products = new List<Product>();
            
            const string connectionString = "Server=127.0.0.1,1433;Database=SMS;User=SA;Password=YourStrong@Passw0rd;TrustServerCertificate=True;";
            //khalid uncomment this one to work ur database vvv and comment this ^^^ (temporary solution)(will probably never change it)
            // string connectionString = "Data Source=DESKTOP-K96CGJS\\SQLEXPRESS;Initial Catalog=SMS;Integrated Security=True;TrustServerCertificate=True";
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
                        ProductImageUrl = reader["ProductImage"] is DBNull ? null : Convert.ToBase64String((byte[])reader["ProductImage"])
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
    }
        
}