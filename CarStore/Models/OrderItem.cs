namespace CarStore.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int CarId { get; set; }
        public Car Car { get; set; }  // Powiązanie z modelem Car
        public decimal Price { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }  // Powiązanie z modelem Order
        public decimal OrderTotal { get; set; }
    }
}
