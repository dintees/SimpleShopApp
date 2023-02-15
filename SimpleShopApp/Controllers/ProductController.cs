using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleShopApp.DAL;
using SimpleShopApp.Models;

namespace SimpleShopApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.categories = await _context.Categories.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product model)
        {
            await _context.AddAsync(model);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(string id)
        {
            // var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == int.Parse(id));
            var product = await _context.Products.Include(e => e.Category).FirstOrDefaultAsync(p => p.Id == int.Parse(id));
            return View(product);
        }

        [Route("/Product/Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == int.Parse(id));
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("/Product/Edit/{id}")]
        public async Task<IActionResult>Edit(string id)
        {
            var product = await _context.Products.Include(e => e.Category).FirstOrDefaultAsync(p => p.Id == int.Parse(id));
            ViewBag.categories = await _context.Categories.ToListAsync();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product model)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == model.Id);
            if (product != null)
            {
                product.Name = model.Name;
                product.Description = model.Description;
                product.Price = model.Price;
                product.Quantity = model.Quantity;
                product.CategoryId = model.CategoryId;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
