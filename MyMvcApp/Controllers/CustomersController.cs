using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMvcApp.Data;
using MyMvcApp.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MyMvcApp.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Customers")]
    public class CustomersController : Controller
    {
        private readonly AppDbContext _ctx;
        public CustomersController(AppDbContext ctx) => _ctx = ctx;

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var customers = await _ctx.Customers
                                      .AsNoTracking()
                                      .ToListAsync();
            return View("~/Views/Admin/Customers/Index.cshtml", customers);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/Customers/Create.cshtml", new Customer());
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Admin/Customers/Create.cshtml", model);

            _ctx.Customers.Add(model);
            await _ctx.SaveChangesAsync();

            TempData["Success"] = "Customer created successfully.";
            return RedirectToAction("Index");
        }

        [HttpGet("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var customer = await _ctx.Customers.FindAsync(id);
            if (customer == null)
            {
                TempData["Error"] = "Customer not found.";
                return RedirectToAction("Index");
            }

            return View("~/Views/Admin/Customers/Edit.cshtml", customer);
        }

        [HttpPost("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Customer model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Admin/Customers/Edit.cshtml", model);

            var customerInDb = await _ctx.Customers.FindAsync(id);
            if (customerInDb == null) return NotFound();

            customerInDb.FirstName = model.FirstName;
            customerInDb.LastName = model.LastName;
            customerInDb.Email = model.Email;
            customerInDb.PhoneNumber = model.PhoneNumber;
            customerInDb.Address = model.Address;
            customerInDb.City = model.City;
            customerInDb.PostalCode = model.PostalCode;
            customerInDb.Gender = model.Gender;

            await _ctx.SaveChangesAsync();
            TempData["Success"] = "Customer updated successfully.";
            return RedirectToAction("Index");
        }

        [HttpGet("Details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var customer = await _ctx.Customers.FindAsync(id);
            if (customer == null) return NotFound();

            return View("~/Views/Admin/Customers/DetailsCustomer.cshtml", customer);
        }

        [HttpPost("Delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _ctx.Customers.FindAsync(id);
            if (customer == null)
            {
                TempData["Error"] = "Customer not found.";
                return RedirectToAction("Index");
            }

            _ctx.Customers.Remove(customer);
            await _ctx.SaveChangesAsync();

            TempData["Success"] = "Customer deleted successfully.";
            return RedirectToAction("Index");
        }

        [HttpGet("GroupsSegments")]
        public IActionResult GroupsSegments()
        {
            return View("~/Views/Admin/Customers/GroupsSegments.cshtml");
        }
    }
}