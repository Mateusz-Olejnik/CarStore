using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarStore.Data;
using CarStore.Models;

namespace CarStore.Controllers
{
    [Authorize] // Autoryzacja dla tylko zalogowanych użytkowników
    public class CarsController : Controller
    {
        private readonly CarStoreContext _context;

        public CarsController(CarStoreContext context)
        {
            _context = context;
        }

        // GET: Cars
        public async Task<IActionResult> Index()
        {
            // restrykcja tylko dla admina ponizej 
            if (User.Identity.Name != "admin@wp.pl")
            {
                return Forbid(); // zwraca 403 niedozwolone jak nie admin 
            }

            return View(await _context.Cars.ToListAsync());
        }

        // GET: Cars/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Cars == null)
            {
                return NotFound();
            }

            var car = await _context.Cars.FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // GET: Cars/Create
        public IActionResult Create()
        {
            if (User.Identity.Name != "admin@wp.pl")
            {
                return Forbid();
            }

            return View();
        }

        // POST: Cars/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Brand,Model,Price,Description,ImageUrl")] Car car)
        {
            if (User.Identity.Name != "admin@wp.pl")
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                _context.Add(car);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(car);
        }

        // GET: Cars/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (User.Identity.Name != "admin@wp.pl")
            {
                return Forbid();
            }

            if (id == null || _context.Cars == null)
            {
                return NotFound();
            }

            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Cars/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Brand,Model,Price,Description,ImageUrl")] Car car)
        {
            if (User.Identity.Name != "admin@wp.pl")
            {
                return Forbid();
            }

            if (id != car.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(car);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(car);
        }

        // GET: Cars/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (User.Identity.Name != "admin@wp.pl")
            {
                return Forbid();
            }

            if (id == null || _context.Cars == null)
            {
                return NotFound();
            }

            var car = await _context.Cars.FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Cars/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (User.Identity.Name != "admin@wp.pl")
            {
                return Forbid();
            }

            if (_context.Cars == null)
            {
                return Problem("Entity set 'CarStoreContext.Cars' is null.");
            }

            var car = await _context.Cars.FindAsync(id);
            if (car != null)
            {
                _context.Cars.Remove(car);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }
    }
}
