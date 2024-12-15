namespace CarStore.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; } // UserId aby śledzić użytkownika który składa zamówienie
          public DateTime OrderPlaced { get; set; } // dodanie daty obecnej w basket controller jest datetime.now()
        public decimal OrderTotal { get; set; }
    
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
