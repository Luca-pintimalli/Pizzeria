using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pizzeria.Models;
using Pizzeria.Services;

namespace Pizzeria.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext _dataContext;

        public AccountController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _dataContext.Users
                    .Include(u => u.UsersRoles)
                    .ThenInclude(ur => ur.Role)
                    .SingleOrDefault(u => u.Email == model.Email && u.Password == model.Password);

                if (user == null)
                {
                    TempData["Error"] = "Email o password non validi.";
                    return View(model);
                }
                else
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Email)
                    };

                    // Aggiungi i ruoli come claim
                    var roles = user.UsersRoles.Select(ur => ur.Role.Nome).ToArray();
                    TempData["Roles"] = roles; // Assicurati che i ruoli siano memorizzati in TempData

                    claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }




    }
}