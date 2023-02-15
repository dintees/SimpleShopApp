using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleShopApp.DAL;
using SimpleShopApp.Models;

namespace SimpleShopApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.ToListAsync();

            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category model)
        {
            if (ModelState.IsValid)
            {
                var category = new Category()
                {
                    Name = model.Name
                };
                await _context.Categories.AddAsync(category);
                _context.SaveChanges();
                TempData["successMessage"]= "Category <strong>" + model.Name + "</strong> has been added.";
                return RedirectToAction("Index");
            } else
            {
                return View();
            }
        }

        [HttpGet]
        [Route("/Category/Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == int.Parse(id));
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                TempData["successMessage"] = "Category <strong>" + category.Name + "</strong> has been deleted.";
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("/Category/Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == int.Parse(id));
            if (category != null)
            {
                var model = new Category();
                model.Id = category.Id;
                model.Name = category.Name;
                return await Task.Run(() => View("Edit", model));
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> Edit(Category model)
        {
            var category = await _context.Categories.FindAsync(model.Id);
            if (category != null)
            {
                // category.Id = model.Id;
                category.Name = model.Name;

                await _context.SaveChangesAsync();
                TempData["successMessage"] = "Category <strong>" + category.Name + "</strong> has been edited.";
            }
            return RedirectToAction("Index");
        }

    }
}
