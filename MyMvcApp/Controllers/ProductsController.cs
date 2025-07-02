using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMvcApp.Data;
using MyMvcApp.Models;
using System.Threading.Tasks;

namespace MyMvcApp.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Products")]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _ctx;
        public ProductsController(AppDbContext ctx) => _ctx = ctx;

    [HttpGet("")]
    [HttpGet("Index")]
    public async Task<IActionResult> Index(string searchName, string category, bool? hasDiscount)
    {
        var products = _ctx.Products.AsQueryable();

        if (!string.IsNullOrEmpty(searchName))
            products = products.Where(p => p.ProductName.Contains(searchName));

        if (!string.IsNullOrEmpty(category))
            products = products.Where(p => p.Category.Contains(category));

        if (hasDiscount.HasValue)
        {
            if (hasDiscount.Value)
                products = products.Where(p => p.Discount.HasValue && p.Discount.Value > 0);
            else
                products = products.Where(p => !p.Discount.HasValue || p.Discount.Value == 0);
        }

        var list = await products.AsNoTracking().ToListAsync();
        return View("~/Views/Admin/Products/Index.cshtml", list);
    }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/Products/Create.cshtml", new Product());
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Admin/Products/Create.cshtml", model);

            _ctx.Products.Add(model);
            await _ctx.SaveChangesAsync();

            TempData["Success"] = "Product created successfully.";
            return RedirectToAction("Index");
        }

        [HttpGet("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _ctx.Products.FindAsync(id);
            if (product == null)
            {
                TempData["Error"] = "Product not found.";
                return RedirectToAction("Index");
            }

            return View("~/Views/Admin/Products/UpdateProducts.cshtml", product);
        }

        [HttpPost("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Admin/Products/UpdateProducts.cshtml", model);

            var productInDb = await _ctx.Products.FindAsync(id);
            if (productInDb == null) return NotFound();

            productInDb.ProductName = model.ProductName;
            productInDb.Description = model.Description;
            productInDb.Price = model.Price;
            productInDb.Discount = model.Discount;
            productInDb.StockQuantity = model.StockQuantity;
            productInDb.Category = model.Category;
            productInDb.ImageUrl = model.ImageUrl;
            productInDb.IsActive = model.IsActive;

            await _ctx.SaveChangesAsync();
            TempData["Success"] = "Product updated successfully.";
            return RedirectToAction("Index");
        }

        [HttpGet("Details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _ctx.Products.FindAsync(id);
            if (product == null) return NotFound();

            return View("~/Views/Admin/Products/ProductsDetail.cshtml", product);
        }

        [HttpPost("Delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _ctx.Products.FindAsync(id);
            if (product == null)
            {
                TempData["Error"] = "Product not found.";
                return RedirectToAction("Index");
            }

            _ctx.Products.Remove(product);
            await _ctx.SaveChangesAsync();

            TempData["Success"] = "Product deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
