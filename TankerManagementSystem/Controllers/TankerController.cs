using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TankerManagementSystem.Models;

namespace TankerManagementSystem.Controllers
{
    public class TankerController : Controller
    {
        private readonly TankerDbContext _dbcontext;

        public TankerController(TankerDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        // LIST
        public IActionResult Index()
        {
            var admin = HttpContext.Session.GetString("admin_session");
            if (admin == null) return RedirectToAction("Login", "Admin");

            var tankers = _dbcontext.Tankers
                .Include(x => x.Owner)
                .OrderByDescending(x => x.Id)
                .ToList();

            return View(tankers);
        }

        // ADD GET
        public IActionResult Add()
        {
            var admin = HttpContext.Session.GetString("admin_session");
            if (admin == null) return RedirectToAction("Login", "Admin");

            ViewBag.Owners = _dbcontext.TankerOwners.ToList();
            return View();
        }

        // ADD POST
        [HttpPost]
        public IActionResult Add(Tanker request)
        {
            var pakistanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            request.CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, pakistanTimeZone);

            var sessionValue = HttpContext.Session.GetString("admin_session");

            if (!string.IsNullOrEmpty(sessionValue))
                request.CreatedBy = int.Parse(sessionValue);
            else
                return RedirectToAction("Login", "Admin");

            if (string.IsNullOrWhiteSpace(request.TankerNo))
            {
                TempData["Error"] = "Tanker Number is required";
                return RedirectToAction("Add");
            }

            _dbcontext.Tankers.Add(request);
            _dbcontext.SaveChanges();

            TempData["add_tanker_message"] = "Tanker added successfully.";
            return RedirectToAction("Index");
        }

        // EDIT GET
        public IActionResult Edit(int id)
        {
            var admin = HttpContext.Session.GetString("admin_session");
            if (admin == null) return RedirectToAction("Login", "Admin");

            var tanker = _dbcontext.Tankers.FirstOrDefault(x => x.Id == id);
            if (tanker == null) return NotFound();

            ViewBag.Owners = _dbcontext.TankerOwners.ToList();
            return View(tanker);
        }

        // EDIT POST
        [HttpPost]
        public IActionResult Edit(Tanker updateTanker)
        {
            var tanker = _dbcontext.Tankers.FirstOrDefault(x => x.Id == updateTanker.Id);
            if (tanker == null) return NotFound();

            var pakistanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            tanker.UpdatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, pakistanTimeZone);

            tanker.TankerNo = updateTanker.TankerNo;
            tanker.OwnerId = updateTanker.OwnerId;
            tanker.Model = updateTanker.Model;
            tanker.Capacity = updateTanker.Capacity;
            tanker.PreviousBalance = updateTanker.PreviousBalance;

            var sessionValue = HttpContext.Session.GetString("admin_session");

            if (!string.IsNullOrEmpty(sessionValue))
                tanker.UpdatedBy = int.Parse(sessionValue);
            else
                return RedirectToAction("Login", "Admin");

            _dbcontext.SaveChanges();

            TempData["edit_tanker_message"] = "Tanker updated successfully.";
            return RedirectToAction("Index");
        }
    }
}