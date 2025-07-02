using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMvcApp.Data;
using MyMvcApp.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using MyMvcApp.ViewModels;

namespace MyMvcApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> LoginUser(string email, string password)
        {
            var user = _context.Users
                .Include(u => u.Role) // Load role information as well
                .FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user == null)
            {
                TempData["UserLoginError"] = "Invalid email or password.";
                return RedirectToAction("Login");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim("FullName", $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, user.Role?.Name ?? "User")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("FullName", $"{user.FirstName} {user.LastName}");
            HttpContext.Session.SetString("Role", user.Role?.Name ?? "User");

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Register(string firstName, string lastName, string email, string password, string passwordRepeat)
        {
            if (password != passwordRepeat)
            {
                TempData["RegisterError"] = "Passwords do not match.";
                return RedirectToAction("Login");
            }

            if (_context.Users.Any(u => u.Email == email))
            {
                TempData["RegisterError"] = "This email is already registered.";
                return RedirectToAction("Login");
            }

            var defaultRole = _context.Roles.FirstOrDefault(r => r.Name == "User");
            if (defaultRole == null)
            {
                TempData["RegisterError"] = "Default user role not found.";
                return RedirectToAction("Login");
            }

            var newUser = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Password = password,
                UserName = email,
                RoleId = defaultRole.Id
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            TempData["RegisterSuccess"] = "Registration successful!";
            return RedirectToAction("Login");
        }

        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login");

            var user = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return RedirectToAction("Login");

            return View(user);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
