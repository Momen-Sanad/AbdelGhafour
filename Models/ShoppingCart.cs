using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuperMarket.Models
{
    public class ShoppingCart
    {
        [Key]
        [Column(Order = 1)]
        public int CartID { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        public string Content { get; set; }

        [Required]
        [Column(Order = 2)]
        public int CustomerID { get; set; }
    }
}