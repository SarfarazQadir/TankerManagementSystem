using Microsoft.AspNetCore.Mvc;
using TankerManagementSystem.Models;

namespace TankerManagementSystem.Controllers
{
    public class AdminController : Controller
    {
        private TankerDbContext _mycon;
        private IWebHostEnvironment _env;
        public AdminController(TankerDbContext mycon, IWebHostEnvironment env)
        {
            _mycon = mycon;
            _env = env;
        }
        public IActionResult Index()
        {
            var admin = HttpContext.Session.GetString("admin_session");
            if (admin == null) return RedirectToAction("Login", "Admin");
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string adminEmail, string adminPassword)
        {
            var row = _mycon.tbl_admin.FirstOrDefault(a => a.admin_email == adminEmail);
            if (row != null && row.admin_password == adminPassword)
            {
                HttpContext.Session.SetString("admin_session", row.admin_id.ToString());
                TempData["login_message"] = "Login successfull...";
                return RedirectToAction("Index","Admin");
            }
            else
            {
                TempData["login_failed_message"] = "Incorrect email or password!";
                return View();
            }
        }

        public IActionResult Profile()
        {
            var admin = HttpContext.Session.GetString("admin_session");
            if (admin != null)
            {
                var adm = HttpContext.Session.GetString("admin_session");
                var data = _mycon.tbl_admin.Where(a => a.admin_id == int.Parse(adm)).ToList();
                return View(data);
            }
            else
            {
                return RedirectToAction("login");
            }

        }
        [HttpPost]
        public IActionResult Profile(Admin admin1)
        {
            var admin = HttpContext.Session.GetString("admin_session");
            if (admin == null) return RedirectToAction("Login", "Admin");
            _mycon.tbl_admin.Update(admin1);
            _mycon.SaveChanges();
            return RedirectToAction("profile");
        }
        [HttpPost]
        public IActionResult ChangeProfileImage(IFormFile admin_image, Admin admin)
        {
            string ImagePath = Path.Combine(_env.WebRootPath, "Adminassests", "AdminImages", admin_image.FileName);
            string directoryPath = Path.Combine(_env.WebRootPath, "Adminassests", "AdminImages");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            try
            {
                using (FileStream fs = new FileStream(ImagePath, FileMode.Create))
                {
                    admin_image.CopyTo(fs);
                }
                admin.admin_image = admin_image.FileName;
                _mycon.tbl_admin.Update(admin);
                _mycon.SaveChanges();
                return RedirectToAction("profile");
            }
            catch (Exception ex)
            {
                // Log or handle the error
                Console.WriteLine($"Error saving image: {ex.Message}");
            }
            return RedirectToAction("profile");

        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("admin_session");
            TempData["logout_message"] = "Logout successfully...";
            return RedirectToAction("login");
        }
    }
}
