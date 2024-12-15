using CarStore.Data;
using CarStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CarStore.Controllers
{
    [Authorize]
    public class BasketController : Controller
    {
        private readonly CarStoreContext _context;

        public BasketController(CarStoreContext context)
        {
            _context = context;
        }

        // wyswietla basket items 
        public async Task<IActionResult> Index()
        {
            var userId = User.Identity.Name; // pobiera id zalogowanego

            var basketItems = await _context.BasketItems
                .Where(b => b.UserId == userId)
                .Include(b => b.Car)
                .ToListAsync();

            ViewBag.TotalPrice = basketItems.Sum(b => b.Quantity * b.Car.Price);

            return View(basketItems);
        }

        // Pokazywanie szczegółów zamówienia w podsumowaniu
        public async Task<IActionResult> Checkout()
        {
            var userId = User.Identity.Name;

            var basketItems = await _context.BasketItems
                .Where(b => b.UserId == userId)
                .Include(b => b.Car)
                .ToListAsync();

            var totalPrice = basketItems.Sum(b => b.Quantity * b.Car.Price);

            var checkoutViewModel = new CheckoutViewModel
            {
                BasketItems = basketItems,
                TotalPrice = totalPrice,
                Order = new Order
                {
                    UserId = userId,
                    OrderPlaced = DateTime.Now,
                    OrderTotal = totalPrice
                }
            };

            return View(checkoutViewModel);
        }

        // Składanie zamówienia i przejście do podsumowania
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.Name;

                // nowe zamowienie order
                var order = new Order
                {
                    UserId = userId,
                    OrderPlaced = DateTime.Now,
                    OrderTotal = model.TotalPrice
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // tworzenie  order items z basket items
                foreach (var basketItem in model.BasketItems)
                {
                    var orderItem = new OrderItem
                    {
                        CarId = basketItem.CarId,
                        OrderId = order.Id,
                        Quantity = basketItem.Quantity,
                        OrderTotal = basketItem.Quantity * basketItem.Car.Price
                    };

                    _context.OrderItems.Add(orderItem);
                }

                // zapisz order items w bazie danych
                await _context.SaveChangesAsync();

                // usuwanie przedmiotow z koszyka po tym jak zamowienie jest zrobione
                var basketItemsToRemove = _context.BasketItems.Where(b => b.UserId == userId).ToList();
                _context.BasketItems.RemoveRange(basketItemsToRemove);
                await _context.SaveChangesAsync();

                // przekierowanie do potwierdzenia zamówienia
                return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
            }

            // zwraca bledy jezeli model nie jest poprawny
            return View("Checkout", model);
        }

        // orderconfirmation.cshtml w views/order po zlozeniu zamowienia
        public IActionResult OrderConfirmation(int orderId)
        {
            var order = _context.Orders
                                .Include(o => o.OrderItems)
                                .ThenInclude(oi => oi.Car)
                                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order); //  zwrócenie szczegółów zamówienia do widoku potwierdzenia
        }

        // dodanie car to koszyka
        [HttpPost]
        public async Task<IActionResult> AddToBasket(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            var userId = User.Identity.Name;

            var basket = await _context.Baskets.FirstOrDefaultAsync(b => b.UserId == userId);

            if (basket == null)
            {
                basket = new Basket { UserId = userId };
                _context.Baskets.Add(basket);
                await _context.SaveChangesAsync();
            }

            var basketItem = await _context.BasketItems
                .FirstOrDefaultAsync(b => b.CarId == car.Id && b.UserId == userId);

            if (basketItem == null)
            {
                basketItem = new BasketItem
                {
                    CarId = car.Id,
                    BasketId = basket.Id,
                    UserId = userId,
                    Quantity = 1
                };

                _context.BasketItems.Add(basketItem);
            }
            else
            {
                basketItem.Quantity++;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // zwieksza ilość samochodu o 1
        [HttpPost]
        public async Task<IActionResult> IncreaseQuantity(int id)
        {
            var basketItem = await _context.BasketItems
                .FirstOrDefaultAsync(b => b.CarId == id && b.UserId == User.Identity.Name);

            if (basketItem != null)
            {
                basketItem.Quantity++;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // zmniejsza ilość o 1
        [HttpPost]
        public async Task<IActionResult> ReduceQuantity(int id)
        {
            var basketItem = await _context.BasketItems
                .FirstOrDefaultAsync(b => b.CarId == id && b.UserId == User.Identity.Name);

            if (basketItem != null && basketItem.Quantity > 0)
            {
                basketItem.Quantity--;
                if (basketItem.Quantity == 0)
                {
                    _context.BasketItems.Remove(basketItem);
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // usuwa z koszyka
        [HttpPost]
        public async Task<IActionResult> RemoveFromBasket(int id)
        {
            var basketItem = await _context.BasketItems
                .FirstOrDefaultAsync(b => b.CarId == id && b.UserId == User.Identity.Name);

            if (basketItem != null)
            {
                _context.BasketItems.Remove(basketItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // oproznia koszyk
        [HttpPost]
        public async Task<IActionResult> ClearBasket()
        {
            var userId = User.Identity.Name;
            var basketItems = await _context.BasketItems
                .Where(b => b.UserId == userId)
                .ToListAsync();

            _context.BasketItems.RemoveRange(basketItems);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
