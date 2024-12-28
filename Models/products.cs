using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuperMarket.Models
{
    public class Product
        {
            public string ProductID { get; set; }
            public string ProductName { get; set; }
            public decimal Price { get; set; }
            public string Category { get; set; }
            public string InventoryID { get; set; }
            public string ProductImageUrl { get; set; }
        }
}
