using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TankerManagementSystem.Models;

public class UserController : Controller
{
    private readonly TankerDbContext _db;

    public UserController(TankerDbContext db)
    {
        _db = db;
    }

    // FETCH USERS
    public IActionResult FetchUsers()
    {
        var admin = HttpContext.Session.GetString("admin_session");
        if (admin == null) return RedirectToAction("Login", "Admin");

        return View(_db.Users.OrderByDescending(x => x.Id).ToList());
    }

    // CREATE GET
    public IActionResult CreatUser()
    {
        var admin = HttpContext.Session.GetString("admin_session");
        if (admin == null) return RedirectToAction("Login", "Admin");

        return View();
    }

    // CREATE POST
    [HttpPost]
    public IActionResult CreatUser(User request)
    {
        // UNIQUE CHECK
        if (_db.Users.Any(x => x.Email == request.Email))
        {
            TempData["Error"] = "Email already exists";
            return RedirectToAction("CreatUser");
        }
        request.RoleId = 1;
        if (_db.Users.Any(x => x.UserName == request.UserName))
        {
            TempData["Error"] = "Username already exists";
            return RedirectToAction("CreatUser");
        }

        request.CreatedAt = DateTime.Now;
        request.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.PasswordHash);

        _db.Users.Add(request);
        _db.SaveChanges();

        TempData["success"] = "User Created Successfully";
        return RedirectToAction("FetchUsers");
    }

    // EDIT GET
    public IActionResult EditUser(int id)
    {
        var user = _db.Users.FirstOrDefault(x => x.Id == id);
        if (user == null) return NotFound();

        ViewBag.Modules = _db.AppModules.ToList();
        ViewBag.Permissions = _db.UserPermissions.Where(x => x.UserId == id).ToList();

        return View(user);
    }

    // EDIT POST (WITH PERMISSIONS)
    [HttpPost]
    public IActionResult EditUser(User model, List<UserPermission> permissions)
    {
        var user = _db.Users.FirstOrDefault(x => x.Id == model.Id);
        if (user == null) return NotFound();

        user.FullName = model.FullName;
        user.Email = model.Email;
        user.UserName = model.UserName;

        // PASSWORD CHANGE (OPTIONAL)
        if (!string.IsNullOrWhiteSpace(model.PasswordHash))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.PasswordHash);
        }

        // REMOVE OLD PERMISSIONS
        var oldPermissions = _db.UserPermissions.Where(x => x.UserId == user.Id);
        _db.UserPermissions.RemoveRange(oldPermissions);

        // ADD NEW PERMISSIONS
        foreach (var item in permissions)
        {
            item.UserId = user.Id;
            _db.UserPermissions.Add(item);
        }

        _db.SaveChanges();

        TempData["success"] = "User Updated Successfully";
        return RedirectToAction("FetchUsers");
    }

    // DELETE
    public IActionResult DeleteUser(int id)
    {
        var user = _db.Users.FirstOrDefault(x => x.Id == id);
        if (user == null) return NotFound();

        _db.Users.Remove(user);
        _db.SaveChanges();

        return RedirectToAction("FetchUsers");
    }
}