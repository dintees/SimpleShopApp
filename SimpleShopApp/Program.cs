using Microsoft.EntityFrameworkCore;
using SimpleShopApp.DAL;
using SimpleShopApp.Extensions;
using FluentValidation;
using SimpleShopApp.Models;
using SimpleShopApp.Models.Validators;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
builder.Configuration.GetConnectionString("Connection")));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IValidator<CategoryModel>, CategoryValidator>();
builder.Services.AddScoped<IValidator<ProductModel>, ProductValidator>();

// binder for decimal values
builder.Services.AddControllersWithViews(config =>
{
    config.ModelBinderProviders.Insert(0, new InvariantDecimalModelBinderProvider());
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
