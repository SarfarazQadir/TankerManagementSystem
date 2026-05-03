using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TankerManagementSystem.Models;

namespace TankerManagementSystem.Controllers
{
    public class TripEntryController : Controller
    {
        private readonly TankerDbContext _db;

        public TripEntryController(TankerDbContext db)
        {
            _db = db;
        }

        // LIST
        public IActionResult Index()
        {
            var admin = HttpContext.Session.GetString("admin_session");
            if (admin == null) return RedirectToAction("Login", "Admin");

            var trips = _db.TripEntries
                .Include(x => x.TankerFk)
                .OrderByDescending(x => x.Id)
                .ToList();

            return View(trips);
        }

        // ADD GET
        public IActionResult AddEntry()
        {
            var admin = HttpContext.Session.GetString("admin_session");
            if (admin == null) return RedirectToAction("Login", "Admin");

            ViewBag.Tankers = _db.Tankers.ToList();
            return View();
        }

        // ADD POST
        [HttpPost]
        public IActionResult AddEntry(TripEntry request)
        {
            var pakistanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            request.CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, pakistanTimeZone);

            var sessionValue = HttpContext.Session.GetString("admin_session");

            if (!string.IsNullOrEmpty(sessionValue))
                request.CreatedBy = int.Parse(sessionValue);
            else
                return RedirectToAction("Login", "Admin");

            if (request.TankerId == 0 || string.IsNullOrWhiteSpace(request.ToLocation))
            {
                TempData["Error"] = "Required fields missing";
                return RedirectToAction("AddEntry");
            }

            _db.TripEntries.Add(request);
            _db.SaveChanges();

            TempData["add_trip_message"] = "Trip added successfully.";
            return RedirectToAction("Index");
        }

        // EDIT GET
        public IActionResult EditEntry(int id)
        {
            var admin = HttpContext.Session.GetString("admin_session");
            if (admin == null) return RedirectToAction("Login", "Admin");

            var trip = _db.TripEntries.FirstOrDefault(x => x.Id == id);
            if (trip == null) return NotFound();

            ViewBag.Tankers = _db.Tankers.ToList();
            return View(trip);
        }

        // EDIT POST
        [HttpPost]
        public IActionResult EditEntry(TripEntry update)
        {
            var trip = _db.TripEntries.FirstOrDefault(x => x.Id == update.Id);
            if (trip == null) return NotFound();

            var pakistanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            trip.UpdatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, pakistanTimeZone);

            trip.LoadDate = update.LoadDate;
            trip.TankerId = update.TankerId;
            trip.AdvanceCash = update.AdvanceCash;
            trip.FromLocation = update.FromLocation;
            trip.ToLocation = update.ToLocation;

            var sessionValue = HttpContext.Session.GetString("admin_session");
            if (!string.IsNullOrEmpty(sessionValue))
                trip.UpdatedBy = int.Parse(sessionValue);
            else
                return RedirectToAction("Login", "Admin");

            _db.SaveChanges();

            TempData["edit_trip_message"] = "Trip updated successfully.";
            return RedirectToAction("Index");
        }
    }
}