namespace CarStore.Models
{
    public class CheckoutViewModel
    {
        // przechowuje basket items, czyli produkty ktore dodaje/zamawia uzytkownik 
        public List<BasketItem> BasketItems { get; set; }

        // info o zamowieniu ktore sklada uzytkownik 
        public Order Order { get; set; }

        // cena totalna za wszystkie auta 
        public decimal TotalPrice { get; set; }

        //  przechowuje id nowo utworzonego zamowienia (order)
        public int OrderId { get; set; }
    }
}
