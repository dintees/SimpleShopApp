using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleShopApp.Entities;
using SimpleShopApp.Models;
using FluentValidation;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace SimpleShopApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IValidator<ProductModel> _validator;
        private readonly IMapper _mapper;
        public ProductController(IValidator<ProductModel> validator, IMapper mapper, ApplicationDbContext context)
        {
            _context = context;
            _validator = validator;
            _mapper = mapper;
        }

        // *** READ ***
        public async Task<IActionResult> Index(string search)
        {
            List<Product> products;
            if (search != null) { products = await _context.Products.Include(e => e.Category).Where(p => p.Name.Contains(search)).ToListAsync(); }
            else { products = await _context.Products.Include(e => e.Category).ToListAsync(); }
            // if (category != null) products = products.FindAll(p => p.CategoryId == int.Parse(category));

            var productView = _mapper.Map<IEnumerable<Product>, IEnumerable<ProductModel>>(products);

            return View(productView);
        }

        // *** CREATE ***
        [Authorize]
        public async Task<IActionResult> Create()
        {
            ViewBag.categories = await GetCategoriesAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductModel model)
        {
            var validation = await _validator.ValidateAsync(model);
            if (!validation.IsValid)
            {
                validation.AddToModelState(ModelState);
                //  errors = ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList();
                ViewBag.categories = await GetCategoriesAsync();
                return View(model);
            }
            else
            {
                var product = _mapper.Map<Product>(model);
                await _context.AddAsync(product);
                await _context.SaveChangesAsync();
                TempData["successMessage"] = "Product <strong>" + model.Name + "</strong> has been added.";
                return RedirectToAction("Index");
            }
        }

        // *** DETAILS ***
        public async Task<IActionResult> Details(string id)
        {
            var product = await _context.Products.Include(e => e.Category).FirstOrDefaultAsync(p => p.Id == int.Parse(id));
            //var productView = new ProductModel() { Id = product.Id, Name = product.Name, Description = product.Description, Price = product.Price, Quantity = product.Quantity, CategoryId = product.CategoryId, CategoryName = product.Category.Name };
            var productView = _mapper.Map<ProductModel>(product);
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
            var productView = _mapper.Map<ProductModel>(product);
            ViewBag.categories = await GetCategoriesAsync();
            return View(productView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductModel model)
        {
            var validation = await _validator.ValidateAsync(model);
            if (!validation.IsValid)
            {
                validation.AddToModelState(ModelState);
                ViewBag.categories = await GetCategoriesAsync();
                return View(model);
            }
            else
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == model.Id);
                if (product != null)
                {
                    // product = _mapper.Map<Product>(model);
                    product.Name = model.Name;
                    product.Description = model.Description;
                    product.Price = (decimal)model.Price;
                    product.Quantity = (int)model.Quantity;
                    product.CategoryId = model.CategoryId;
                    await _context.SaveChangesAsync();
                    TempData["successMessage"] = "Product <strong>" + model.Name + "</strong> has been edited.";
                }
                return RedirectToAction("Index");
            }
        }

        private async Task<IEnumerable<CategoryModel>> GetCategoriesAsync()
        {
            var categories = await _context.Categories.ToListAsync();
            return categories.Select(c => new CategoryModel { Id = c.Id, Name = c.Name });
        }
    }
}
