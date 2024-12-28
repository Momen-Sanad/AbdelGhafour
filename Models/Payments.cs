using System.Collections.Generic;

namespace SuperMarket.Models
{
    
    public class Payments
    {

        public int PaymentID { get; set; }  
        public string PaymentMethod { get; set; }
        public string PaymentDate { get; set; }
        public double Amount { get; set; }
    }
}