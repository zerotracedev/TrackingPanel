using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMvcApp.Data;
using MyMvcApp.Models;

public class ProfileController : Controller
{
    private readonly AppDbContext _context;

    public ProfileController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var email = HttpContext.Session.GetString("UserEmail");

        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction("Login", "Auth");
        }

        var user = _context.Users
            .Include(u => u.Role)
            .FirstOrDefault(u => u.Email == email);

        if (user == null)
        {
            return NotFound("Kullanıcı bulunamadı.");
        }

        return View(user);
    }
}
