using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SuperMarket.Models;
using System.Data.SqlClient;


namespace SuperMarket.Pages
{
    public class CartModel : PageModel
    {
        public Cart ShoppingCart { get; set; } = new Cart();
        string connectionString = "Data Source=DESKTOP-K96CGJS\\SQLEXPRESS;Initial Catalog=SMS;Integrated Security=True;TrustServerCertificate=True";
        public void OnGet()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                string query = "SELECT ProductID, ProductName, Price, SUM(Quantity) AS TotalQuantity, ItemImage FROM CartItem WHERE CartID = 1 GROUP BY ProductID, ProductName, Price, ItemImage";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
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
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        public IActionResult OnPostDeleteItem(int productId)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                string query = "DELETE FROM CartItem WHERE CartID = 1 AND ProductID = @ProductId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductId", productId);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                connection.Close();
            }

            return RedirectToPage();
        }

    }
}
