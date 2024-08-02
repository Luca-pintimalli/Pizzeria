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
            // Inizializzo il logger e il contesto dati tramite dependency injection
            _logger = logger;
            _dataContext = dataContext;
        }

        public IActionResult Index()
        {
            // Recupero tutti gli articoli dal database, inclusi gli ingredienti associati
            var articoli = _dataContext.Articoli
                                       .Include(a => a.Ingredienti)
                                       .ToList();

            // Ottengo l'email dell'utente attualmente autenticato
            var userEmail = User.Identity?.Name;
            if (userEmail != null)
            {
                // Cerco l'utente nel database tramite l'email
                var user = _dataContext.Users
                    .Include(u => u.UsersRoles)
                    .ThenInclude(ur => ur.Role)
                    .SingleOrDefault(u => u.Email == userEmail);

                if (user != null)
                {
                    // Estraggo i nomi dei ruoli dell'utente e li passo alla vista tramite ViewBag
                    var roles = user.UsersRoles.Select(ur => ur.Role.Nome).ToArray();
                    ViewBag.UserRoles = roles;
                }
            }
            // Passo la lista degli articoli alla vista
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
