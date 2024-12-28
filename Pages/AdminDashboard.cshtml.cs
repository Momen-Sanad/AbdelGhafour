using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace SuperMarket.Pages
{
    [Authorize(Roles="Admin")]
    public class AdminDashboardModel : PageModel
    {
        private readonly string connectionString = "Data Source=DESKTOP-K96CGJS\\SQLEXPRESS;Initial Catalog=SMS;Integrated Security=True;TrustServerCertificate=True";
        public List<Staff> Staff { get; set; } = new List<Staff>();

        [BindProperty]
        public Staff NewStaff { get; set; } = new Staff();

        public bool IsEditing => NewStaff?.StaffID != 0;

        public void OnGet(int? editId)
        {
            LoadStaff();

            if (editId.HasValue)
            {
                NewStaff = Staff.FirstOrDefault(p => p.StaffID == editId.Value) ?? new Staff();
            }
        }

        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid)
            {
                LoadStaff();
                return Page();
            }

            if (NewStaff.StaffID <= 0)
            {
                ModelState.AddModelError("NewStaff.StaffID", "Staff ID must be a positive number.");
                LoadStaff();
                return Page();
            }

            if (!IsStaffIDValid(NewStaff.StaffID))
            {
                ModelState.AddModelError("NewStaff.StaffID", "Staff ID already exists. Please use a unique ID.");
                LoadStaff();
                return Page();
            }

            try
            {
                using var connection = new SqlConnection(connectionString);
                var query = @"INSERT INTO Staff (StaffID, StaffName, StaffRole, StaffPhone, AdminID) 
                             VALUES (@StaffID, @StaffName, @StaffRole, @StaffPhone, @AdminID)";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@StaffID", NewStaff.StaffID);
                command.Parameters.AddWithValue("@StaffName", NewStaff.StaffName);
                command.Parameters.AddWithValue("@StaffRole", NewStaff.StaffRole);
                command.Parameters.AddWithValue("@StaffPhone", NewStaff.StaffPhone);
                command.Parameters.AddWithValue("@AdminID", NewStaff.AdminID);

                connection.Open();
                command.ExecuteNonQuery();

                TempData["SuccessMessage"] = "Staff member added successfully.";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error adding staff member: {ex.Message}");
                LoadStaff();
                return Page();
            }
        }

        public IActionResult OnPostEdit()
        {
            if (!ModelState.IsValid)
            {
                LoadStaff();
                return Page();
            }

            try
            {
                using var connection = new SqlConnection(connectionString);
                var query = @"UPDATE Staff 
                             SET StaffName = @StaffName, 
                                 StaffRole = @StaffRole, 
                                 StaffPhone = @StaffPhone, 
                                 AdminID = @AdminID 
                             WHERE StaffID = @StaffID";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@StaffID", NewStaff.StaffID);
                command.Parameters.AddWithValue("@StaffName", NewStaff.StaffName);
                command.Parameters.AddWithValue("@StaffRole", NewStaff.StaffRole);
                command.Parameters.AddWithValue("@StaffPhone", NewStaff.StaffPhone);
                command.Parameters.AddWithValue("@AdminID", NewStaff.AdminID);

                connection.Open();
                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    ModelState.AddModelError("", "Staff member not found or no changes were made.");
                    LoadStaff();
                    return Page();
                }

                TempData["SuccessMessage"] = "Staff member updated successfully.";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating staff member: {ex.Message}");
                LoadStaff();
                return Page();
            }
        }

        public IActionResult OnPostDelete(int StaffID)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                var query = "DELETE FROM Staff WHERE StaffID = @StaffID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@StaffID", StaffID);

                connection.Open();
                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    TempData["ErrorMessage"] = "Staff member not found.";
                }
                else
                {
                    TempData["SuccessMessage"] = "Staff member deleted successfully.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting staff member: {ex.Message}";
            }

            return RedirectToPage();
        }

        private void LoadStaff()
        {
            Staff.Clear();
            using var connection = new SqlConnection(connectionString);
            var query = "SELECT * FROM Staff ORDER BY StaffName ASC";
            var command = new SqlCommand(query, connection);

            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                Staff.Add(new Staff
                {
                    StaffID = Convert.ToInt32(reader["StaffID"]),
                    StaffName = reader["StaffName"].ToString(),
                    StaffRole = reader["StaffRole"].ToString(),
                    StaffPhone = reader["StaffPhone"].ToString(),
                    AdminID = Convert.ToInt32(reader["AdminID"])
                });
            }
        }

        private bool IsStaffIDValid(int StaffID)
        {
            using var connection = new SqlConnection(connectionString);
            var query = "SELECT COUNT(1) FROM Staff WHERE StaffID = @StaffID";
            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@StaffID", StaffID);

            connection.Open();
            return (int)command.ExecuteScalar() == 0;
        }
    }

    public class Staff
    {
        [Required(ErrorMessage = "Staff ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Staff ID must be a positive number")]
        public int StaffID { get; set; }

        [Required(ErrorMessage = "Staff Name is required")]
        [StringLength(100, ErrorMessage = "Staff Name cannot exceed 100 characters")]
        public string StaffName { get; set; }

        [Required(ErrorMessage = "Staff Role is required")]
        [StringLength(50, ErrorMessage = "Staff Role cannot exceed 50 characters")]
        public string StaffRole { get; set; }

        [Required(ErrorMessage = "Staff Phone is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string StaffPhone { get; set; }

        [Required(ErrorMessage = "Admin ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Admin ID must be a positive number")]
        public int AdminID { get; set; }
    }
}