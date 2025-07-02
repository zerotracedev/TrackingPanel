using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMvcApp.Data;
using MyMvcApp.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyMvcApp.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Stock")]
    public class StockController : Controller
    {
        private readonly AppDbContext _ctx;

        public StockController(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        // Stokta olan ürünlerin listesi (Index, CurrentInventory ile aynı görünümü gösterir)
        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var products = await _ctx.Products
                                     .AsNoTracking()
                                     .ToListAsync();
            // Burada CurrentInventory.cshtml kullanılıyor
            return View("~/Views/Admin/Stock/CurrentInventory.cshtml", products);
        }

        // Mevcut stok durumu
        [HttpGet("CurrentInventory")]
        public async Task<IActionResult> CurrentInventory()
        {
            var products = await _ctx.Products
                                     .AsNoTracking()
                                     .ToListAsync();
            return View("~/Views/Admin/Stock/CurrentInventory.cshtml", products);
        }

        // Düşük stok uyarıları (< 10)
        [HttpGet("LowStockAlerts")]
        public async Task<IActionResult> LowStockAlerts()
        {
            var lowStockProducts = await _ctx.Products
                                             .Where(p => p.StockQuantity < 10)
                                             .AsNoTracking()
                                             .ToListAsync();
            return View("~/Views/Admin/Stock/LowStockAlerts.cshtml", lowStockProducts);
        }

        // Stok ekleme formu (GET)
        [HttpGet("AddProductStock")]
        public IActionResult AddProductStock()
        {
            return View("~/Views/Admin/Stock/AddProductStock.cshtml");
        }

        // Stok ekleme işlemi (POST)
        [HttpPost("AddProductStock")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProductStock(ProductStockUpdateModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Admin/Stock/AddProductStock.cshtml", model);
            }

            var product = await _ctx.Products.FindAsync(model.ProductID);
            if (product == null)
            {
                ModelState.AddModelError("ProductID", "Product not found.");
                return View("~/Views/Admin/Stock/AddProductStock.cshtml", model);
            }

            product.StockQuantity += model.QuantityToAdd;
            await _ctx.SaveChangesAsync();

            TempData["Success"] = $"Stock updated successfully. New quantity: {product.StockQuantity}";
            return RedirectToAction("CurrentInventory");
        }

        // Ürün Detayları (GET)
        [HttpGet("Details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _ctx.Products
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(p => p.ProductID == id);

            if (product == null)
                return NotFound();

            return View("~/Views/Admin/Stock/Details.cshtml", product);
        }

        // Ürün Düzenleme Formu (GET)
        [HttpGet("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _ctx.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            return View("~/Views/Admin/Stock/Edit.cshtml", product);
        }

        // Ürün Düzenleme İşlemi (POST)
        [HttpPost("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product model)
        {
            if (id != model.ProductID)
                return BadRequest();

            if (!ModelState.IsValid)
                return View("~/Views/Admin/Stock/Edit.cshtml", model);

            var product = await _ctx.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            // Güncelleme
            product.ProductName = model.ProductName;
            product.Description = model.Description;
            product.Price = model.Price;
            product.Discount = model.Discount;
            product.StockQuantity = model.StockQuantity;
            product.Category = model.Category;

            await _ctx.SaveChangesAsync();

            TempData["Success"] = "Product updated successfully.";
            return RedirectToAction("Index");
        }

        // Ürün Silme (POST)
        [HttpPost("Delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _ctx.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            _ctx.Products.Remove(product);
            await _ctx.SaveChangesAsync();

            TempData["Success"] = "Product deleted successfully.";
            return RedirectToAction("Index");
        }
    }

    public class ProductStockUpdateModel
    {
        [Required]
        public int ProductID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int QuantityToAdd { get; set; }
    }
}
