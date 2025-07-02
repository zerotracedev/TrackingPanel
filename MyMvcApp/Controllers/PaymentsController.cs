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
    [Route("Admin/Payments")]
    public class PaymentsController : Controller
    {
        private readonly AppDbContext _ctx;

        public PaymentsController(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        // GET: Admin/Payments or Admin/Payments/Index
        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var payments = await _ctx.Payments
                                     .Include(p => p.Order)
                                     .OrderByDescending(p => p.PaymentDate)
                                     .ToListAsync();

            var viewModel = new PaymentIndexViewModel
            {
                Payments = payments,
                TotalCount = payments.Count,
                CompletedCount = payments.Count(p => p.Status == "Completed"),
                PendingCount = payments.Count(p => p.Status == "Pending"),
                FailedCount = payments.Count(p => p.Status == "Failed")
            };

            // Tam view yolu veriyoruz, dosya: Views/Admin/Payments/Index.cshtml
            return View("~/Views/Admin/Payments/Index.cshtml", viewModel);
        }

        // POST: Admin/Payments/SyncPaymentsWithOrders
        [HttpPost("SyncPaymentsWithOrders")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SyncPaymentsWithOrders()
        {
            var paymentOrderIds = await _ctx.Payments.Select(p => p.OrderId).ToListAsync();

            var ordersWithoutPayment = await _ctx.Orders
                .Where(o => !paymentOrderIds.Contains(o.Id))
                .ToListAsync();

            foreach (var order in ordersWithoutPayment)
            {
                var payment = new Payment
                {
                    OrderId = order.Id,
                    Amount = order.TotalAmount,
                    PaymentDate = System.DateTime.Today,
                    PaymentMethod = order.PaymentMethod ?? "Unknown",
                    Status = "Pending"
                };
                _ctx.Payments.Add(payment);
            }

            int added = await _ctx.SaveChangesAsync();
            TempData["Success"] = $"{added} payment record(s) created.";
            return RedirectToAction("Index");
        }

        // GET: Admin/Payments/Details/5
        [HttpGet("Details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var payment = await _ctx.Payments
                                     .Include(p => p.Order)
                                     .AsNoTracking()
                                     .FirstOrDefaultAsync(p => p.Id == id);

            if (payment == null)
                return NotFound();

            return View("~/Views/Admin/Payments/Details.cshtml", payment);
        }

        // GET: Admin/Payments/Edit/5
        [HttpGet("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var payment = await _ctx.Payments
                                     .Include(p => p.Order)
                                     .AsNoTracking()
                                     .FirstOrDefaultAsync(p => p.Id == id);

            if (payment == null)
                return NotFound();

            return View("~/Views/Admin/Payments/Edit.cshtml", payment);
        }

        // POST: Admin/Payments/Edit/5
        [HttpPost("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Payment payment)
        {
            ModelState.Remove("Order");

            if (!ModelState.IsValid)
            {
                payment.Order = await _ctx.Orders
                                           .AsNoTracking()
                                           .FirstOrDefaultAsync(o => o.Id == payment.OrderId);
                return View("~/Views/Admin/Payments/Edit.cshtml", payment);
            }

            var paymentInDb = await _ctx.Payments.FirstOrDefaultAsync(p => p.Id == id);
            if (paymentInDb == null)
                return NotFound();

            paymentInDb.Amount = payment.Amount;
            paymentInDb.PaymentDate = payment.PaymentDate;
            paymentInDb.PaymentMethod = payment.PaymentMethod;
            paymentInDb.Status = payment.Status;

            await _ctx.SaveChangesAsync();

            TempData["Success"] = "Payment updated successfully.";
            return RedirectToAction("Index");
        }
    }
}
