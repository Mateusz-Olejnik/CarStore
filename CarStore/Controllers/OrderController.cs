using CarStore.Data;
using CarStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CarStore.Controllers
{
    public class OrderController : Controller
    {
        private readonly CarStoreContext _context;

        public OrderController(CarStoreContext context)
        {
            _context = context;
        }

        // GET dla strony podsumowania
        [HttpGet]
        public IActionResult Checkout()
        {
            var userId = User.Identity.IsAuthenticated ? User.Identity.Name : "anonymous";
            var basket = GetBasket(userId);
            var basketItems = basket?.BasketItems?.ToList() ?? new List<BasketItem>();

            var checkoutViewModel = new CheckoutViewModel
            {
                BasketItems = basketItems,
                TotalPrice = basketItems.Sum(item => item.Car?.Price * item.Quantity ?? 0)
            };

            return View(checkoutViewModel);
        }

        // POST dla składania zamówienia
        [HttpPost]
        public IActionResult PlaceOrder()
        {
            var userId = User.Identity.IsAuthenticated ? User.Identity.Name : "anonymous";
            var basket = GetBasket(userId);

            // Jeśli koszyk jest pusty -> przenieś do strony koszyka i pokaż błąd
            if (basket == null || !basket.BasketItems.Any())
            {
                TempData["ErrorMessage"] = "Your basket is empty!";
                return RedirectToAction("Index", "Basket");
            }

            // tworzenie nowego zamówienia
            var order = new Order
            {
                UserId = userId,
               OrderPlaced = DateTime.Now,
                OrderTotal = basket.BasketItems.Sum(item => item.Car.Price * item.Quantity),
                OrderItems = basket.BasketItems.Select(item => new OrderItem
                {
                    CarId = item.CarId,
                    Quantity = item.Quantity,
                    OrderTotal = item.Quantity * item.Car.Price
                }).ToList()
            };

            // zapisanie zamówienia do bazy
            _context.Orders.Add(order);

            // czyści koszyk po złożeniu zamówienia
            _context.BasketItems.RemoveRange(basket.BasketItems);

            // opcjonalnie można też samemu wyczyścić koszyk
            _context.Baskets.Remove(basket);

            // zapisanie zmian w bazie i wyczyszczenie koszyka
            _context.SaveChanges();

            // Przekierowanie do strony potwierdzenia
            return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
        }

        // strona podsumowania zamówienia
        public async Task<IActionResult> OrderConfirmation(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Car)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // pobieranie koszyka dla bieżącego użytkownika
        private Basket GetBasket(string userId)
        {
            var basket = _context.Baskets
                                 .Include(b => b.BasketItems)
                                 .ThenInclude(bi => bi.Car)
                                 .FirstOrDefault(b => b.UserId == userId);

            if (basket == null)
            {
                // jeśli koszyk nie istnieje, tworzy nowy
                basket = new Basket { UserId = userId };
                _context.Baskets.Add(basket);
                _context.SaveChanges();
            }

            return basket;
        }
    }
}
