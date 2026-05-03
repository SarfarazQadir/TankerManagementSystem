using Microsoft.AspNetCore.Mvc;
using TankerManagementSystem.Models;
using System.Linq;

namespace TankerManagementSystem.Controllers
{
    public class TankerOwnerController : Controller
    {
        private readonly TankerDbContext _dbcontext;

        public TankerOwnerController(TankerDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        // LIST
        public IActionResult Index()
        {
            var admin = HttpContext.Session.GetString("admin_session");
            if (admin == null) return RedirectToAction("Login", "Admin");

            var owners = _dbcontext.TankerOwners
                .OrderByDescending(x => x.Id)
                .ToList();

            return View(owners);
        }

        // ADD GET
        public IActionResult Add()
        {
            var admin = HttpContext.Session.GetString("admin_session");
            if (admin == null) return RedirectToAction("Login", "Admin");

            return View();
        }

        // ADD POST
        [HttpPost]
        public IActionResult Add(TankerOwner request)
        {
            var pakistanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            request.CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, pakistanTimeZone);

            var sessionValue = HttpContext.Session.GetString("admin_session");

            if (!string.IsNullOrEmpty(sessionValue))
            {
                request.CreatedBy = int.Parse(sessionValue);
            }
            else
            {
                TempData["Error"] = "Session expired. Please login again.";
                return RedirectToAction("Login", "Admin");
            }

            // Basic validation
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                TempData["Error"] = "Owner Name is required";
                return RedirectToAction("Add");
            }

            _dbcontext.TankerOwners.Add(request);
            _dbcontext.SaveChanges();

            TempData["add_owner_message"] = "Tanker Owner added successfully.";
            return RedirectToAction("Index");
        }

        // EDIT GET
        public IActionResult Edit(int id)
        {
            var admin = HttpContext.Session.GetString("admin_session");
            if (admin == null) return RedirectToAction("Login", "Admin");

            var owner = _dbcontext.TankerOwners.FirstOrDefault(x => x.Id == id);

            if (owner == null)
                return NotFound();

            return View(owner);
        }

        // EDIT POST
        [HttpPost]
        public IActionResult Edit(TankerOwner updateOwner)
        {
            var owner = _dbcontext.TankerOwners.FirstOrDefault(x => x.Id == updateOwner.Id);

            if (owner == null)
                return NotFound();

            // Optional: Duplicate CNIC check
            var duplicate = _dbcontext.TankerOwners
                .Any(x => x.CNIC == updateOwner.CNIC && x.Id != updateOwner.Id);

            if (duplicate)
            {
                ModelState.AddModelError("CNIC", "CNIC must be unique.");
                return View(updateOwner);
            }

            var pakistanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            owner.UpdatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, pakistanTimeZone);

            owner.Name = updateOwner.Name;
            owner.Phone = updateOwner.Phone;
            owner.CNIC = updateOwner.CNIC;
            owner.Address = updateOwner.Address;

            var sessionValue = HttpContext.Session.GetString("admin_session");

            if (!string.IsNullOrEmpty(sessionValue))
            {
                owner.UpdatedBy = int.Parse(sessionValue);
            }
            else
            {
                TempData["Error"] = "Session expired. Please login again.";
                return RedirectToAction("Login", "Admin");
            }

            _dbcontext.SaveChanges();

            TempData["edit_owner_message"] = "Tanker Owner updated successfully.";
            return RedirectToAction("Index");
        }
    }
}