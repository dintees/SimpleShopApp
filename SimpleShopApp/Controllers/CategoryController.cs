using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleShopApp.Entities;
using SimpleShopApp.Models;

namespace SimpleShopApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IValidator<CategoryModel> _validator;
        private readonly IMapper _mapper;

        public CategoryController(IValidator<CategoryModel> validator, IMapper mapper, ApplicationDbContext context)
        {
            _context = context;
            _validator = validator;
            _mapper = mapper;
        }

        // *** READ ***
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.ToListAsync();
            // var categoriesView = categories.Select(c => new CategoryModel { Id = c.Id, Name = c.Name });
            var categoriesView = _mapper.Map<IEnumerable<Category>, IEnumerable<CategoryModel>>(categories);

            return View(categoriesView);
        }

        // *** CREATE ***
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryModel model)
        {
            var validation = await _validator.ValidateAsync(model);
            if (validation.IsValid)
            {
                var category = new Category() { Name = model.Name };
                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();
                TempData["successMessage"] = "Category <strong>" + model.Name + "</strong> has been added.";
                return RedirectToAction("Index");
            }
            else
            {
                validation.AddToModelState(ModelState);
                return View(model);
            }
        }

        // *** DELETE ***
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

        // *** UPDATE ***
        [HttpGet]
        [Route("/Category/Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == int.Parse(id));
            if (category != null)
            {
                var categoryView = _mapper.Map<CategoryModel>(category);
                return View(categoryView);
                // return await Task.Run(() => View("Edit", model));

            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryModel model)
        {
            var validation = await _validator.ValidateAsync(model);
            if (!validation.IsValid)
            {
                validation.AddToModelState(ModelState);
                return View(model);
            }
            else
            {
                var category = await _context.Categories.FindAsync(model.Id);
                if (category != null)
                {
                    category.Name = model.Name;
                    await _context.SaveChangesAsync();
                    TempData["successMessage"] = "Category <strong>" + category.Name + "</strong> has been edited.";
                }
                return RedirectToAction("Index");
            }
        }
    }
}
