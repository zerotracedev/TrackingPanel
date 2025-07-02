using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMvcApp.Data;
using MyMvcApp.Models;
using MyMvcApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MyMvcApp.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Orders")]
    public class OrdersController : Controller
    {
        private readonly AppDbContext _ctx;
        public OrdersController(AppDbContext ctx) => _ctx = ctx;

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(string customerName, DateTime? dateFrom, DateTime? dateTo, string status)
        {
            var query = _ctx.Orders
                .Include(o => o.Customer)
                .AsNoTracking()
                .OrderByDescending(o => o.OrderDate)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(customerName))
            {
                query = query.Where(o => (o.Customer.FirstName + " " + o.Customer.LastName).Contains(customerName));
            }

            if (dateFrom.HasValue)
            {
                query = query.Where(o => o.OrderDate >= dateFrom.Value);
            }

            if (dateTo.HasValue)
            {
                query = query.Where(o => o.OrderDate <= dateTo.Value);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(o => o.Status == status);
            }

            var list = await query.ToListAsync();

            return View("~/Views/Admin/Orders/Index.cshtml", list);
        }

        [HttpGet("OrderDetail/{id:int}")]
        public async Task<IActionResult> OrderDetail(int id)
        {
            var order = await _ctx.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            return View("~/Views/Admin/Orders/OrderDetail.cshtml", order);
        }

        [HttpGet("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _ctx.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            return View("~/Views/Admin/Orders/Edit.cshtml", order);
        }

        [HttpPost("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order model)
        {
            if (id != model.Id)
                return BadRequest();

            var order = await _ctx.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
                return NotFound();

            order.Status = model.Status;
            order.OrderDate = model.OrderDate;
            order.TotalAmount = model.TotalAmount;
            order.PhoneNumber = model.PhoneNumber;
            order.Email = model.Email;
            order.ShippingAddress = model.ShippingAddress;

            try
            {
                await _ctx.SaveChangesAsync();
                TempData["Success"] = "Order was updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error occurred during update: " + ex.Message;
                return View("~/Views/Admin/Orders/Edit.cshtml", model);
            }

            return RedirectToAction("OrderDetail", new { id = order.Id });
        }

        [HttpGet("NewOrder")]
        public async Task<IActionResult> NewOrder()
        {
            var customers = await _ctx.Customers.ToListAsync();
            var products = await _ctx.Products.Where(p => p.IsActive).ToListAsync();

            var items = new List<OrderItemViewModel>
            {
                new OrderItemViewModel { Products = products, Quantity = 1 }
            };

            var vm = new OrderCreateViewModel
            {
                OrderDate = DateTime.Today,
                Customers = customers,
                Items = items
            };
            return View("~/Views/Admin/Orders/NewOrder.cshtml", vm);
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderCreateViewModel vm)
        {
            vm.Customers = await _ctx.Customers.ToListAsync();
            var products = await _ctx.Products.Where(p => p.IsActive).ToListAsync();

            foreach (var item in vm.Items)
                item.Products = products;

            if (vm.Items == null || !vm.Items.Any())
                ModelState.AddModelError("Items", "You must select at least one product.");

            foreach (var item in vm.Items ?? Enumerable.Empty<OrderItemViewModel>())
            {
                if (!products.Any(p => p.ProductID == item.ProductID))
                    ModelState.AddModelError("Items", $"Selected product (ID: {item.ProductID}) not found.");
            }

            if (!ModelState.IsValid)
                return View("~/Views/Admin/Orders/NewOrder.cshtml", vm);

            decimal total = 0;
            foreach (var item in vm.Items)
            {
                var product = products.FirstOrDefault(p => p.ProductID == item.ProductID);
                item.UnitPrice = product.Price;
                total += item.Quantity * item.UnitPrice;
            }
            vm.TotalAmount = total;

            var order = new Order
            {
                OrderNumber = $"ORD-{DateTime.Now:yyyyMMddHHmmss}",
                CustomerID = vm.CustomerID,
                OrderDate = vm.OrderDate,
                ShippingAddress = vm.ShippingAddress,
                TotalAmount = vm.TotalAmount,
                OrderItems = vm.Items.Select(i => new OrderItem
                {
                    ProductID = i.ProductID,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList(),
                Status = "Pending"
            };

            _ctx.Orders.Add(order);

            try
            {
                await _ctx.SaveChangesAsync();

                if (order.Id > 0)
                {
                    var newPayment = new Payment
                    {
                        OrderId = order.Id,
                        Amount = order.TotalAmount,
                        PaymentDate = DateTime.Today,
                        PaymentMethod = "N/A",
                        Status = "Pending"
                    };
                    _ctx.Payments.Add(newPayment);
                    await _ctx.SaveChangesAsync();
                }

                TempData["Success"] = "Order was created successfully.";
            }
            catch (DbUpdateException dbEx)
            {
                var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                ModelState.AddModelError("", "Database error occurred during save: " + innerMessage);
                return View("~/Views/Admin/Orders/NewOrder.cshtml", vm);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An unexpected error occurred: " + ex.Message);
                return View("~/Views/Admin/Orders/NewOrder.cshtml", vm);
            }

            return RedirectToAction("OrderDetail", new { id = order.Id });
        }

        // Delete
        [HttpPost("Delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _ctx.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == id);

            if (order != null)
            {
                var cargoTrackings = await _ctx.CargoTrackings.Where(ct => ct.OrderId == id).ToListAsync();
                if (cargoTrackings.Any())
                    _ctx.CargoTrackings.RemoveRange(cargoTrackings);

                var payments = await _ctx.Payments.Where(p => p.OrderId == id).ToListAsync();
                if (payments.Any())
                    _ctx.Payments.RemoveRange(payments);

                _ctx.OrderItems.RemoveRange(order.OrderItems);
                _ctx.Orders.Remove(order);
                await _ctx.SaveChangesAsync();
                TempData["Success"] = "Order and related records have been deleted.";
            }
            else
            {
                TempData["Error"] = "Order not found or already deleted.";
            }

            return RedirectToAction("Index");
        }
    }
}
