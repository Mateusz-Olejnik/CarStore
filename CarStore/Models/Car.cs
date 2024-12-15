using System.ComponentModel.DataAnnotations;

namespace CarStore.Models
{
    public class Car
    {
        public int Id { get; set; }
        [Required,
         MaxLength(100)]
        public string Brand { get; set; }
        [Required,
         MaxLength(100)]
        public string Model { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Range(1, int.MaxValue, ErrorMessage = "Cena musi być liczbą dodatnią!")]
        public int Price { get; set; }

        [Required,
         MaxLength(300)]
        public string Description { get; set; }
        
        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; } 
    }
}
