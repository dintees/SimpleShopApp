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

        // *** READ ***
        public async Task<IActionResult> Index(string search, string category)
        {
            List<Product> products;
            if (search != null) { products = await _context.Products.Where(p => p.Name.Contains(search)).ToListAsync(); }
            else { products = await _context.Products.ToListAsync(); }
            if (category != null) products = products.FindAll(p => p.CategoryId == int.Parse(category));

            var productView = products.Select(p => new ProductModel() { Id = p.Id, Name = p.Name, Description = p.Description, Price = p.Price, Quantity = p.Quantity, CategoryId = p.CategoryId });

            return View(productView);
        }

        // *** CREATE ***
        public async Task<IActionResult> Create()
        {
            ViewBag.categories = await GetCategories();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductModel model)
        {

            if (ModelState.IsValid)
            {
                var product = new Product()
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    Quantity = model.Quantity,
                    CategoryId = model.CategoryId,
                };

                await _context.AddAsync(product);
                await _context.SaveChangesAsync();
                TempData["successMessage"] = "Product <strong>" + model.Name + "</strong> has been added.";
                return RedirectToAction("Index");
            } else
            {
                var errors = ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList();
            }
            return RedirectToAction("Index");
        }

        // *** DETAILS ***
        public async Task<IActionResult> Details(string id)
        {
            var product = await _context.Products.Include(e => e.Category).FirstOrDefaultAsync(p => p.Id == int.Parse(id));
            var productView = new ProductModel() { Id = product.Id, Name = product.Name, Description = product.Description, Price = product.Price, Quantity = product.Quantity, CategoryId = product.CategoryId, CategoryName = product.Category.Name };
            return View(productView);
        }

        // *** DELETE ***
        [Route("/Product/Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == int.Parse(id));
            if (product != null)
            {
                _context.Products.Remove(product);
                TempData["successMessage"] = "Product <strong>" + product.Name + "</strong> has been deleted.";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // *** UPDATE ***
        [HttpGet]
        [Route("/Product/Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var product = await _context.Products.Include(e => e.Category).FirstOrDefaultAsync(p => p.Id == int.Parse(id));
            var productView = new ProductModel() { Id = product.Id, Name = product.Name, Description = product.Description, Price = product.Price, Quantity = product.Quantity, CategoryId = product.CategoryId };
            ViewBag.categories = await GetCategories();
            return View(productView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductModel model)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == model.Id);
            if (product != null)
            {
                product.Name = model.Name;
                product.Description = model.Description;
                product.Price = model.Price;
                product.Quantity = model.Quantity;
                product.CategoryId = model.CategoryId;
                await _context.SaveChangesAsync();
                TempData["successMessage"] = "Product <strong>" + model.Name + "</strong> has been edited.";
            }
            return RedirectToAction("Index");
        }

        private async Task<IEnumerable<CategoryModel>> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            return categories.Select(c => new CategoryModel { Id = c.Id, Name = c.Name });
        }
    }
}
