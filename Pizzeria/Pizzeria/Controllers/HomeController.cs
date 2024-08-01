using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pizzeria.Models;
using Pizzeria.Services;
using System.Linq;

namespace Pizzeria.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext _dataContext;

        public HomeController(ILogger<HomeController> logger, DataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }

        public IActionResult Index()
        {
            var articoli = _dataContext.Articoli
                                       .Include(a => a.Ingredienti) // Include gli ingredienti 
                                       .ToList();

            // Ottieni il ruolo dell'utente attualmente autenticato
            var userEmail = User.Identity?.Name;
            if (userEmail != null)
            {
                var user = _dataContext.Users
                    .Include(u => u.UsersRoles)
                    .ThenInclude(ur => ur.Role)
                    .SingleOrDefault(u => u.Email == userEmail);

                if (user != null)
                {
                    var roles = user.UsersRoles.Select(ur => ur.Role.Nome).ToArray();
                    ViewBag.UserRoles = roles; // Passa i ruoli alla vista tramite ViewBag
                }
            }
            return View(articoli);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
