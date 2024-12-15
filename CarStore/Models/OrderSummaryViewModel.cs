namespace CarStore.Models
{
    public class OrderItemViewModel
    {
        public string Brand { get; set; }
        public string ImageUrl { get; set; }
        public string Model { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class OrderSummaryViewModel
    {
        public List<OrderItemViewModel> OrderItems { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
