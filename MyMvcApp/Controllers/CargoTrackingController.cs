using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMvcApp.Data;
using MyMvcApp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MyMvcApp.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/CargoTracking")]
    public class CargoTrackingController : Controller
    {
        private readonly AppDbContext _ctx;

        public CargoTrackingController(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var list = await _ctx.CargoTrackings
                .Include(x => x.Order)
                .ToListAsync();
            return View();
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            ViewBag.Orders = _ctx.Orders.ToList();
            return View("~/Views/Admin/CargoTracking/Create.cshtml");
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CargoTracking model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Orders = _ctx.Orders.ToList();
                return View("~/Views/Admin/CargoTracking/Create.cshtml", model);
            }

            _ctx.CargoTrackings.Add(model);
            await _ctx.SaveChangesAsync();
            TempData["Success"] = "Cargo tracking added successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}