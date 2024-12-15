using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarStore.Models
{
    public class BasketItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CarId { get; set; }

        //  Nawigacja bo powiazanym car
        [ForeignKey("CarId")]
        public Car Car { get; set; }

        [Required]
        public int BasketId { get; set; }

        // Nawigacja bo powiazanym basket
        [ForeignKey("BasketId")]
        public Basket Basket { get; set; }

       
        [Required]
        public string UserId { get; set; }

        public int Quantity { get; set; } 
    }
}
