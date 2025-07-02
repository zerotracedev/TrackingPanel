using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyMvcApp.Controllers
{
    [Route("User")]
    public class UserController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("Profile")]
        public IActionResult Profile()
        {
            return View("~/Views/User/Profile.cshtml");
        }

        [HttpGet("Orders")]
        public IActionResult Orders()
        {
            return View("~/Views/User/Orders.cshtml");
        }

        [HttpGet("Support")]
        public IActionResult Support()
        {
            return View("~/Views/User/Support.cshtml");
        }

        [HttpGet("ChangePassword")]
        public IActionResult ChangePassword()
        {
            return View("~/Views/User/ChangePassword.cshtml");
        }
        public IActionResult AccessDenied()
        {
            return View("~/Views/User/AccessDenied.cshtml");
        }

    }
}
