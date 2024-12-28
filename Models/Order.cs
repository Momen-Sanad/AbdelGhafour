namespace SuperMarket.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int CustomerID { get; set; }
        public decimal TotalAmount { get; set; }
        public int CartID { get; set; }
        public int PaymentID { get; set; }
    }

}
