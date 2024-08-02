using System;
using Microsoft.AspNetCore.Mvc;
using Pizzeria.Models;
using Pizzeria.Services;

namespace Pizzeria.Controllers
{
    public class IngredienteController : Controller
    {
        private readonly ILogger<IngredienteController> _logger;
        private readonly DataContext _dataContext;

        public IngredienteController(DataContext dataContext, ILogger<IngredienteController> logger)
        {
            // Inizializzo il logger e il contesto dati tramite dependency injection
            _logger = logger;
            _dataContext = dataContext;
        }

        // Metodo per visualizzare la lista degli ingredienti ordinati per nome
        public IActionResult Ingredienti()
        {
            var ingredientiOrdinati = _dataContext.Ingredienti.OrderBy(i => i.Nome).ToList();
            return View(ingredientiOrdinati);
        }

        // Metodo per visualizzare la vista di creazione di un nuovo ingrediente
        public IActionResult NuovoIngrediente()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Metodo per gestire la creazione di un nuovo ingrediente
        public IActionResult NuovoIngrediente(Ingrediente ingrediente)
        {
            if (ModelState.IsValid)
            {
                // Aggiungo il nuovo ingrediente al contesto dati e salvo le modifiche
                _dataContext.Ingredienti.Add(ingrediente);
                _dataContext.SaveChanges();
                return RedirectToAction("NuovoArticolo", "Articolo");
            }
            return View("Index");
        }

        [HttpGet]
        // Metodo per visualizzare la vista di modifica di un ingrediente esistente
        public IActionResult Edit(int id)
        {
            // Trovo l'ingrediente per id
            var ingrediente = _dataContext.Ingredienti.Single(a => a.Id == id);
            return View(ingrediente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Metodo per gestire la modifica di un ingrediente esistente
        public IActionResult Edit(Ingrediente ingrediente)
        {
            if (ModelState.IsValid)
            {
                // Trovo l'ingrediente da aggiornare per id e aggiorno il nome
                var a = _dataContext.Ingredienti.Single(a => a.Id == ingrediente.Id);
                a.Nome = ingrediente.Nome;
                _dataContext.SaveChanges();
                return RedirectToAction("NuovoArticolo", "Articolo");
            }
            return View(ingrediente);
        }

        // Metodo per visualizzare la vista di cancellazione di un ingrediente esistente
        public IActionResult Delete(int id)
        {
            // Trovo l'ingrediente per id
            var ingrediente = _dataContext.Ingredienti.Single(a => a.Id == id);
            return View(ingrediente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Metodo per gestire la cancellazione di un ingrediente
        public IActionResult Delete(Ingrediente ingrediente)
        {
            // Trovo l'ingrediente per id, lo rimuovo dal contesto dati e salvo le modifiche
            var a = _dataContext.Ingredienti.Single(a => a.Id == ingrediente.Id);
            _dataContext.Ingredienti.Remove(a);
            _dataContext.SaveChanges();
            return RedirectToAction("NuovoArticolo", "Articolo");
        }
    }
}


