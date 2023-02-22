using Microsoft.EntityFrameworkCore;
using SimpleShopApp.Entities;
using SimpleShopApp.Extensions;
using FluentValidation;
using SimpleShopApp.Models;
using SimpleShopApp.Models.Validators;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
builder.Configuration.GetConnectionString("Connection")));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IValidator<CategoryModel>, CategoryValidator>();
builder.Services.AddScoped<IValidator<ProductModel>, ProductValidator>();
builder.Services.AddScoped<IValidator<UserModel>, UserValidator>();

// binder for decimal values
builder.Services.AddControllersWithViews(config =>
{
    config.ModelBinderProviders.Insert(0, new InvariantDecimalModelBinderProvider());
});


// Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.LoginPath = "/Login";
        options.Cookie.Name = "auth_cookie";
        options.AccessDeniedPath = "/Home/Forbidden";
    });


/*builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})

// Adding JWTBearer
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        // ValidateAudience = true,
        // ValidAudience = authenticationSettings["ValidAudinece"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:JwtKey"]))
    };
});*/


// disable default model binder
/*builder.Services.AddMvc(options => options.ModelValidatorProviders.Clear());
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});*/

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
