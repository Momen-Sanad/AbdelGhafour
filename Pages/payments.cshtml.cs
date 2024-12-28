using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SuperMarket.Pages
{
    public class PaymentsModel : PageModel
    {
        [BindProperty]
        public PaymentForm PaymentDetails { get; set; }

        public OrderSummary Summary { get; private set; }

        public IActionResult OnGet()
        {
            // Retrieve order summary from TempData
            if (TempData["Subtotal"] != null && TempData["Tax"] != null && TempData["Shipping"] != null && TempData["Total"] != null)
            {
                Summary = new OrderSummary
                {
                    Subtotal = (decimal)TempData["Subtotal"],
                    Tax =      (decimal)TempData["Tax"],
                    Shipping = (decimal)TempData["Shipping"],
                    Total =    (decimal)TempData["Total"]
                };

                TempData.Keep(); // Keep TempData for postbacks
            }
            else
            {
                return RedirectToPage("/Cart"); // Redirect back to the cart if no summary data is available
            }

            PaymentDetails = new PaymentForm();
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                // Re-populate the summary since TempData is not preserved across requests
                Summary = new OrderSummary
                {
                    Subtotal = (decimal)TempData["Subtotal"],
                    Tax =      (decimal)TempData["Tax"],
                    Shipping = (decimal)TempData["Shipping"],
                    Total =    (decimal)TempData["Total"]
                };
                TempData.Keep();
                return Page();
            }

            // Process payment details (mock logic for now)
            TempData["SuccessMessage"] = "Payment completed successfully!";
            return RedirectToPage("/Success"); // Navigate to a success page
        }

        public class PaymentForm
        {
            public string CardName { get; set; }
            public string CardNumber { get; set; }
            public string ExpiryDate { get; set; }
            public string CVV { get; set; }
            public string BillingAddress { get; set; }
        }

        public class OrderSummary
        {
            public decimal Subtotal { get; set; }
            public decimal Tax { get; set; }
            public decimal Shipping { get; set; }
            public decimal Total { get; set; }
        }
    }
}
