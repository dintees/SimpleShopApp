using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SimpleShopApp.Models;
using System.Security.Claims;
using SimpleShopApp.Entities;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace SimpleShopApp.Controllers
{
    public class UserController : Controller
    {
        /*        private readonly IConfiguration _configuration;
                public UserController(IConfiguration configuration)
                {
                    _configuration = configuration;
                }*/
        private readonly ApplicationDbContext _context;
        private readonly IValidator<UserModel> _validator;
        public UserController(IValidator<UserModel> validator, ApplicationDbContext context)
        {
            _validator = validator;
            _context = context;
        }

        [Route("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserModel model)
        {
            var loggedUser = _context.Users.Include(e => e.Role).FirstOrDefault(u => u.Username.Equals(model.Username) && u.Password.Equals(model.Password));
            if (loggedUser != null)
            {
                var claims = new List<Claim>() {
                    new Claim(ClaimTypes.NameIdentifier, loggedUser.Id.ToString()),
                    new Claim(ClaimTypes.Name, loggedUser.Username),
                    new Claim(ClaimTypes.Role, loggedUser.Role.Name)
                };

                var user = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(user));
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.errorMessage = "Bad username or password.";
                return View(model);
            }
        }

        [HttpGet("Register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("Register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserModel model)
        {
            var validation = await _validator.ValidateAsync(model);
            if (!validation.IsValid)
            {
                validation.AddToModelState(ModelState);
                return View(model);
            }
            else
            {
                var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username || u.Email == model.Email);
                if (userInDb != null)
                {
                    ViewBag.errorMessage = "There is a user with the given username or email address";
                    return View(model);
                }
                else
                {
                    var user = new User
                    {
                        Username = model.Username,
                        Password = model.Password,
                        Email = model.Email,
                        RoleId = 2 // administrator - static for now
                    };
                    await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();

                    var claims = new List<Claim>() {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Role, "Administrator") // static for now
                    };

                    var userClaims = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(userClaims));
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }




        /*        private string GenerateJwtToken()
                {
                    var claims = new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "1"),
                        new Claim(ClaimTypes.Name, "Test"),
                        new Claim(ClaimTypes.Role, "Administrator")
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:JwtKey"]));
                    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var expires = DateTime.Now.AddDays(_configuration.GetValue<int>("JWT:JwtExpireDays"));
                    var token = new JwtSecurityToken(_configuration["JWT:ValidIssuer"], _configuration["JWT:ValidIssuer"],
                        claims,
                        expires: expires,
                        signingCredentials: credentials);
                    return new JwtSecurityTokenHandler().WriteToken(token);
                }*/
    }
}
