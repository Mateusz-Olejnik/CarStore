using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarStore.Data;
using CarStore.Models;
using Microsoft.AspNetCore.Authorization;

namespace CarStore.Controllers
{
    // kontroler odpowiedzialny za wyświetlanie sklepu oraz obsługę
    [Authorize] // sprawdzenie czy użytkownik jest zalogowany
    public class StoreController : Controller
    {
        private readonly CarStoreContext _context;

        public StoreController(CarStoreContext context)
        {
            _context = context;
        }

        // wyświetlanie listy samochodów z opcjonalnym filtrem
        public async Task<IActionResult> Index(string brandFilter, decimal? minPrice, decimal? maxPrice)
        {
            IQueryable<Car> cars = _context.Cars;

            // filtrowanie na podstawie wpisanych parametrów
            if (!string.IsNullOrEmpty(brandFilter))
            {
                cars = cars.Where(c => c.Brand.Contains(brandFilter));
            }

            if (minPrice.HasValue && minPrice.Value >= 0)
            {
                cars = cars.Where(c => c.Price >= minPrice);
            }

            if (maxPrice.HasValue && maxPrice.Value >= 0)
            {
                cars = cars.Where(c => c.Price <= maxPrice);
            }

            // wykonanie i zwrócenie wyfiltrowanych pojazdów
            var carList = await cars.ToListAsync();
            return View(carList);
        } 

        // pokazanie szczegółów samochodu
        public IActionResult Details(int id)
        {
            // łączenie auta z jego id
            var car = _context.Cars.FirstOrDefault(c => c.Id == id);

            // jeśli auto nie istnieje, zwraca komunikat
            if (car == null)
            {
                return NotFound();
            }

            // powrót do widoku szczegółów
            return View(car);
        }
    }
}
