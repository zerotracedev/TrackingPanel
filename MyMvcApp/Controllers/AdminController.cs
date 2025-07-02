using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMvcApp.Data;
using MyMvcApp.ViewModels;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;


namespace MyMvcApp.Controllers
{
    [Route("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;

        public AdminController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            var today = DateTime.Today;

            var latestOrdersRaw = _db.Orders
                .Include(o => o.Customer)
                .OrderByDescending(o => o.OrderDate)
                .Take(10)
                .ToList();

            var latestOrders = latestOrdersRaw
                .Select(o => new OrderSummary
                {
                    OrderNumber = o.OrderNumber,
                    CustomerName = o.Customer != null ? o.Customer.FirstName + " " + o.Customer.LastName : "",
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status
                }).ToList();

            var model = new DashboardViewModel
            {
                TotalOrders = _db.Orders.Count(),
                TotalRevenue = _db.Payments.Sum(p => (decimal?)p.Amount) ?? 0,
                TotalCustomers = _db.Customers.Count(),
                TodayOrders = _db.Orders.Count(o => o.OrderDate.Date == today),
                OrderStatusCounts = _db.Orders
                    .GroupBy(o => o.Status)
                    .Select(g => new OrderStatusCount { Status = g.Key, Count = g.Count() })
                    .ToList(),
                CargoStatusCounts = _db.CargoTrackings
                    .GroupBy(c => c.Status)
                    .Select(g => new CargoStatusCount { Status = g.Key, Count = g.Count() })
                    .ToList(),
                LatestOrders = latestOrders
            };

            return View(model);
        }

        [HttpGet("CargoTracking/Index")]
        public IActionResult CargoTracking()
        {
            return View("~/Views/Admin/CargoTracking/Index.cshtml");
        }

        [HttpGet("Error")]
        public IActionResult Error()
        {
            return View("Error");
        }
    }
}