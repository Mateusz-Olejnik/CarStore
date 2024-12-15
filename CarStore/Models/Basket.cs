using CarStore.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CarStore.Models
{
    public class Basket
    {


        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(450)] // przechowanie userid albo inne identyfikatory dla uzytkownika
        public string UserId { get; set; }

        // przemieszczanie sie po basket items 
        public ICollection<BasketItem> BasketItems { get; set; } = new List<BasketItem>();

        public decimal GetBasketTotal()
        {
            return BasketItems.Sum(item => item.Car.Price * item.Quantity);
        }
    }


}