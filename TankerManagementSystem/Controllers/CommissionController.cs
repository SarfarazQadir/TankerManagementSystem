using Microsoft.AspNetCore.Mvc;
using TankerManagementSystem.Models;
using System.Linq;

namespace TankerManagementSystem.Controllers
{
    public class CommissionController : Controller
    {
        private readonly TankerDbContext _dbcontext;

        public CommissionController(TankerDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        // LIST
        public IActionResult Index()
        {
            var admin = HttpContext.Session.GetString("admin_session");
            if (admin == null) return RedirectToAction("Login", "Admin");

            var data = _dbcontext.CommissionSetups
                .OrderByDescending(x => x.Id)
                .ToList();

            return View(data);
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
        public IActionResult Add(CommissionSetup request)
        {
            var pakistanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            request.CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, pakistanTimeZone);

            var sessionValue = HttpContext.Session.GetString("admin_session");

            if (!string.IsNullOrEmpty(sessionValue))
                request.CreatedBy = int.Parse(sessionValue);
            else
                return RedirectToAction("Login", "Admin");

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                TempData["Error"] = "Commission Name is required";
                return RedirectToAction("Add");
            }

            _dbcontext.CommissionSetups.Add(request);
            _dbcontext.SaveChanges();

            TempData["add_commission_message"] = "Commission added successfully.";
            return RedirectToAction("Index");
        }

        // EDIT GET
        public IActionResult Edit(int id)
        {
            var admin = HttpContext.Session.GetString("admin_session");
            if (admin == null) return RedirectToAction("Login", "Admin");

            var data = _dbcontext.CommissionSetups.FirstOrDefault(x => x.Id == id);
            if (data == null) return NotFound();

            return View(data);
        }

        // EDIT POST
        [HttpPost]
        public IActionResult Edit(CommissionSetup update)
        {
            var data = _dbcontext.CommissionSetups.FirstOrDefault(x => x.Id == update.Id);
            if (data == null) return NotFound();

            var pakistanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            data.UpdatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, pakistanTimeZone);

            data.Name = update.Name;
            data.Percentage = update.Percentage;
            data.IsActive = update.IsActive;

            var sessionValue = HttpContext.Session.GetString("admin_session");

            if (!string.IsNullOrEmpty(sessionValue))
                data.UpdatedBy = int.Parse(sessionValue);
            else
                return RedirectToAction("Login", "Admin");

            _dbcontext.SaveChanges();

            TempData["edit_commission_message"] = "Commission updated successfully.";
            return RedirectToAction("Index");
        }
    }
}