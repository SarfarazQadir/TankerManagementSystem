using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TankerManagementSystem.Models;

namespace TankerManagementSystem.Controllers
{
    public class ProductsController : Controller
    {
        private readonly TankerDbContext _dbcontext;
        public ProductsController(TankerDbContext dbcontext) => _dbcontext = dbcontext;
        public IActionResult Index()
        {
            var admin = HttpContext.Session.GetString("admin_session");
            if (admin == null) return RedirectToAction("Login", "Admin");
            else
            {
                return View(_dbcontext.Products.OrderByDescending(x=>x.Id).ToList());
            }
        }
        public IActionResult Add()
        {
            var admin = HttpContext.Session.GetString("admin_session");
            if (admin == null) return RedirectToAction("Login", "Admin");
            return View();
        }
        [HttpPost]
        public IActionResult Add(Product request)
        {
            // Pakistan Time
            var pakistanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            request.CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, pakistanTimeZone);

            // Session se ID lo
            var sessionValue = HttpContext.Session.GetString("admin_session");

            if (!string.IsNullOrEmpty(sessionValue))
            {
                request.CreatedBy = int.Parse(sessionValue);
            }
            else
            {
                TempData["Error"] = "Session expired. Please login again.";
                return RedirectToAction("Login");
            }

            // Description null handling
            if (string.IsNullOrWhiteSpace(request.Description))
            {
                request.Description = null;
            }

            // Model validation
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Product Name is required";
                return RedirectToAction("Add");
            }

            _dbcontext.Products.Add(request);
            _dbcontext.SaveChanges();
            TempData["add_Product_message"] = "Product add successfully.";

            return RedirectToAction("Index");
        }
        public IActionResult Edit(int id)
        {
            var admin = HttpContext.Session.GetString("admin_session");
            if (admin == null) return RedirectToAction("Login", "Admin");
            var product = _dbcontext.Products.FirstOrDefault(s => s.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        [HttpPost]
        public IActionResult Edit(Product updateproduct)
        {
            var product = _dbcontext.Products.FirstOrDefault(s => s.Id == updateproduct.Id);
            if (product == null)
            {
                return NotFound();
            }

            // Optional: Unique name check
            var duplicate = _dbcontext.Products.Any(s => s.ProductName == updateproduct.ProductName && s.Id != updateproduct.Id);
            if (duplicate)
            {
                ModelState.AddModelError("Name", "Product name must be unique.");
                return View(updateproduct);
            }
            // Pakistan Time
            var pakistanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            product.UpdatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, pakistanTimeZone);

            product.ProductName = updateproduct.ProductName;
            product.Description = updateproduct.Description;
            // Session se ID lo
            var sessionValue = HttpContext.Session.GetString("admin_session");
            if (!string.IsNullOrEmpty(sessionValue))
            {
                product.UpdatedBy = int.Parse(sessionValue);
            }
            else
            {
                TempData["Error"] = "Session expired. Please login again.";
                return RedirectToAction("Login");
            }
            _dbcontext.SaveChanges();

            TempData["edit_Product_message"] = "Product edit successfully.";
            return RedirectToAction("Index");
        }
    }
}
