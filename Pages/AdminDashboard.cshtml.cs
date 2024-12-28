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
        public List<Staff> Staff { get; set; } = new List<Staff>();
        [BindProperty]
        public Staff NewStaff { get; set; }

        public bool IsEditing => NewStaff?.StaffID != null && NewStaff.StaffID != 0;

        public void OnGet(int? editId)
        {
            LoadStaff();

            if (editId.HasValue)
            {
                NewStaff = Staff.FirstOrDefault(p => Convert.ToInt32(p.StaffID) == editId.Value);
            }
            else
            {
                NewStaff = new Staff(); // Initialize a new Staff for adding
            }
        }

        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid)
            {
                LoadStaff();
                return Page();
            }

            int StaffID = NewStaff.StaffID;
            if (StaffID <= 0)
            {
                ModelState.AddModelError("", "Staff ID must be a valid integer.");
                LoadStaff();
                return Page();
            }

            bool isValidStaffID = IsStaffIDValid(StaffID);

            if (isValidStaffID)
            {
                ModelState.AddModelError("", "Staff ID already exists. Please use a unique Staff ID.");
                LoadStaff();
                return Page();
            }

            string connectionString = "Data Source=DESKTOP-K96CGJS\\SQLEXPRESS;Initial Catalog=SMS;Integrated Security=True;TrustServerCertificate=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO Staff
                                (StaffID, StaffName, StaffRole, StaffPhone, AdminID)
                                VALUES 
                                (@StaffID, @StaffName, @StaffRole, @StaffPhone, @AdminID)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@StaffID", StaffID);
                command.Parameters.AddWithValue("@StaffName", NewStaff.StaffName);
                command.Parameters.AddWithValue("@StaffRole", NewStaff.StaffRole);
                command.Parameters.AddWithValue("@StaffPhone", NewStaff.StaffPhone);
                command.Parameters.AddWithValue("@AdminID", NewStaff.StaffPhone);

                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostEdit(int StaffID)
        {
            if (!ModelState.IsValid)
            {
                LoadStaff();
                return Page();
            }

              string connectionString = "Data Source=DESKTOP-K96CGJS\\SQLEXPRESS;Initial Catalog=SMS;Integrated Security=True;TrustServerCertificate=True";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Staff SET StaffName = @StaffName, StaffRole = @StaffRole, StaffPhone = @StaffPhone, AdminID = @AdminID WHERE StaffID = @StaffID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@StaffName", NewStaff.StaffName);
                    command.Parameters.AddWithValue("@StaffRole", NewStaff.StaffRole);
                    command.Parameters.AddWithValue("@StaffPhone", NewStaff.StaffPhone);
                    command.Parameters.AddWithValue("@AdminID", NewStaff.AdminID);

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

        public IActionResult OnPostDelete(string StaffID)
        {
            string connectionString = "Data Source=DESKTOP-K96CGJS\\SQLEXPRESS;Initial Catalog=SMS;Integrated Security=True;TrustServerCertificate=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Staff WHERE StaffID = @StaffID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@StaffID", StaffID);

                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToPage();
        }

        private void LoadStaff()
        {
            string connectionString = "Data Source=DESKTOP-K96CGJS\\SQLEXPRESS;Initial Catalog=SMS;Integrated Security=True;TrustServerCertificate=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Staff ORDER BY StaffName ASC";
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Staff.Add(new Staff
                    {
                        StaffID = int.Parse(reader["StaffID"].ToString()),
                        StaffName = reader["StaffName"].ToString(),
                        StaffRole = reader["StaffRole"].ToString(),
                        StaffPhone = reader["StaffPhone"].ToString(),
                        AdminID = int.Parse(reader["AdminID"].ToString()), 
                    });
                }
            }
        }

        private bool IsStaffIDValid(int StaffID)
        {
            string connectionString = "Data Source=DESKTOP-K96CGJS\\SQLEXPRESS;Initial Catalog=SMS;Integrated Security=True;TrustServerCertificate=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                const string query = "SELECT COUNT(1) FROM Staff WHERE StaffID = @StaffID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@StaffID", StaffID);
                connection.Open();
                return (int)command.ExecuteScalar() > 0;
            }
        }
    }

    public class Staff
    {
        public int StaffID       { get; set; }
        public string StaffName  { get; set; }
        public string StaffRole  { get; set; }
        public string StaffPhone { get; set; }
        public int AdminID       { get; set; }
    }
}