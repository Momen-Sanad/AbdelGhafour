using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuperMarket.Models
{
    public class Customer
    {
        public int CustomerID { get; set; }  
        public string Name { get; set; } 
        public string Email { get; set; }
        public string CustomerPhone { get; set; } 
        public string CustomerAddress { get; set; } 
    }
}
