using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SuperMarket.Pages
{
    public class ProductsModel : PageModel
    {
        public List<Product> Products { get; set; }

        public void OnGet()
        {
            Products = new List<Product>();
            string connectionString = "Data Source=DESKTOP-K96CGJS\\SQLEXPRESS;Initial Catalog=SMS;Integrated Security=True;TrustServerCertificate=True";
            SqlConnection connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();
                string GetAllProducts = "SELECT * FROM Products";
                SqlCommand command = new SqlCommand(GetAllProducts, connection);
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
                    }
                    );
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
