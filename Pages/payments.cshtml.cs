using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace SuperMarket.Pages
{
    [Authorize]
    public class PaymentsModel : PageModel
    {
        [BindProperty]
        public PaymentForm PaymentDetails { get; set; }

        public OrderSummary Summary { get; private set; }

        public IActionResult OnGet()
        {
            if (TempData["Subtotal"] != null && TempData["Tax"] != null &&
            TempData["Shipping"] != null && TempData["Total"] != null)
            {
                decimal subtotal = decimal.Parse(TempData["Subtotal"].ToString());
                decimal taxRate = 0.14m; 
                decimal shipping = 45m; 

                decimal taxAmount = (subtotal * taxRate); 
                decimal total = subtotal + shipping + taxAmount;

                // Populate the OrderSummary
                Summary = new OrderSummary
                {
                    Subtotal = subtotal,
                    Tax = taxAmount,
                    Shipping = shipping,
                    Total = total
                };

                TempData.Keep(); // Keep TempData across requests
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
                    Subtotal = TempData["Subtotal"] != null ? decimal.Parse(TempData["Subtotal"].ToString()) : 0m,
                    Tax = TempData["Tax"] != null ? decimal.Parse(TempData["Tax"].ToString()) : 0m,
                    Shipping = TempData["Shipping"] != null ? decimal.Parse(TempData["Shipping"].ToString()) : 0m,
                    Total = TempData["Total"] != null ? decimal.Parse(TempData["Total"].ToString()) : 0m
                };
                TempData.Keep();
                return Page();
            }

            // Mock payment processing
            bool isPaymentSuccessful = ProcessPayment(PaymentDetails);

            if (isPaymentSuccessful)
            {
                TempData["SuccessMessage"] = "Payment completed successfully!";
                return RedirectToPage("/Success"); // Navigate to a success page
            }

            ModelState.AddModelError(string.Empty, "Payment failed. Please check your details and try again.");
            return Page();
        }


        private bool ProcessPayment(PaymentForm paymentDetails)
        {
            // Mock logic for payment processing (e.g., integrating with a payment gateway API)
            if (!string.IsNullOrWhiteSpace(paymentDetails.CardName) &&
                !string.IsNullOrWhiteSpace(paymentDetails.CardNumber) &&
                !string.IsNullOrWhiteSpace(paymentDetails.ExpiryDate) &&
                !string.IsNullOrWhiteSpace(paymentDetails.CVV) &&
                !string.IsNullOrWhiteSpace(paymentDetails.BillingAddress))
            {
                return true; // Simulate successful payment
            }
            return false; // Simulate payment failure
        }

        public class PaymentForm
        {
            [Required(ErrorMessage = "Cardholder name is required")]
            [Display(Name = "Cardholder Name")]
            public string CardName { get; set; }

            [Required(ErrorMessage = "Card number is required")]
            [CreditCard(ErrorMessage = "Invalid card number")]
            [Display(Name = "Card Number")]
            public string CardNumber { get; set; }

            [Required(ErrorMessage = "Expiry date is required")]
            [RegularExpression(@"^(0[1-9]|1[0-2])\/\d{2}$", ErrorMessage = "Expiry date must be in MM/YY format")]
            [Display(Name = "Expiry Date (MM/YY)")]
            public string ExpiryDate { get; set; }

            [Required(ErrorMessage = "CVV is required")]
            [RegularExpression(@"^\d{3,4}$", ErrorMessage = "Invalid CVV")]
            [Display(Name = "CVV")]
            public string CVV { get; set; }

            [Required(ErrorMessage = "Billing address is required")]
            [Display(Name = "Billing Address")]
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